using System;
using System.IO;
using System.Text.Json;
using LaserCad.Core.Documents;
using LaserCad.ViewportContract;

namespace LaserCad.Desktop;

/// <summary>
/// Prosty klient IPC zapisujacy komunikaty viewportu jako JSON lines dla procesu Unity.
/// </summary>
public sealed class ViewportIpcClient
{
    private readonly DocumentSerializer documentSerializer = new();
    private readonly string outboxPath;
    private readonly string inboxPath;

    public ViewportIpcClient(string? outboxPath = null, string? inboxPath = null)
    {
        var ipcDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LaserCad");

        this.outboxPath = outboxPath ?? Path.Combine(
            ipcDirectory,
            "viewport-outbox.jsonl");
        this.inboxPath = inboxPath ?? Path.Combine(ipcDirectory, "viewport-inbox.jsonl");
    }

    public string OutboxPath => outboxPath;

    public string InboxPath => inboxPath;

    public void SendDocument(CadDocument document)
    {
        var snapshot = new ViewportDocumentSnapshot(documentSerializer.Serialize(document));
        AppendMessage(ViewportMessageKind.DocumentSnapshot, snapshot);
    }

    public void SendViewCommand(ViewportViewCommand command, bool? enabled = null)
    {
        AppendMessage(ViewportMessageKind.ViewCommand, new ViewportViewCommandMessage(command, enabled));
    }

    public ViewportSelectionChangedMessage? ReadLatestSelectionChanged()
    {
        if (!File.Exists(inboxPath))
        {
            return null;
        }

        ViewportSelectionChangedMessage? latestSelection = null;

        foreach (var line in File.ReadLines(inboxPath))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var envelope = JsonSerializer.Deserialize<ViewportEnvelope>(line);
            if (envelope == null || envelope.Kind != ViewportMessageKind.SelectionChanged)
            {
                continue;
            }

            latestSelection = envelope.Payload.Deserialize<ViewportSelectionChangedMessage>();
        }

        return latestSelection;
    }

    private void AppendMessage<TPayload>(ViewportMessageKind kind, TPayload payload)
    {
        var directory = Path.GetDirectoryName(outboxPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var payloadElement = JsonSerializer.SerializeToElement(payload);
        var envelope = new ViewportEnvelope(kind, payloadElement);
        File.AppendAllText(outboxPath, JsonSerializer.Serialize(envelope) + Environment.NewLine);
    }
}
