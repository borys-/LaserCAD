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

    public ViewportIpcClient(string? outboxPath = null)
    {
        this.outboxPath = outboxPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LaserCad",
            "viewport-outbox.jsonl");
    }

    public string OutboxPath => outboxPath;

    public void SendDocument(CadDocument document)
    {
        var snapshot = new ViewportDocumentSnapshot(documentSerializer.Serialize(document));
        AppendMessage(ViewportMessageKind.DocumentSnapshot, snapshot);
    }

    public void SendViewCommand(ViewportViewCommand command, bool? enabled = null)
    {
        AppendMessage(ViewportMessageKind.ViewCommand, new ViewportViewCommandMessage(command, enabled));
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
