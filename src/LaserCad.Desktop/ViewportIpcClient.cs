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
    private long inboxPosition;

    public ViewportIpcClient(string? outboxPath = null, string? inboxPath = null)
    {
        var ipcDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LaserCad");

        this.outboxPath = outboxPath ?? Path.Combine(
            ipcDirectory,
            "viewport-outbox.jsonl");
        this.inboxPath = inboxPath ?? Path.Combine(ipcDirectory, "viewport-inbox.jsonl");

        if (File.Exists(this.inboxPath))
        {
            inboxPosition = new FileInfo(this.inboxPath).Length;
        }
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

    public void SendDrawingTool(ViewportDrawingTool tool)
    {
        AppendMessage(ViewportMessageKind.DrawingToolChanged, new ViewportDrawingToolChangedMessage(tool));
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

    public IReadOnlyList<ViewportShapeDrawnMessage> ReadPendingShapeDrawn()
    {
        var messages = new List<ViewportShapeDrawnMessage>();

        foreach (var envelope in ReadPendingInboxEnvelopes())
        {
            if (envelope.Kind != ViewportMessageKind.ShapeDrawn)
            {
                continue;
            }

            var message = envelope.Payload.Deserialize<ViewportShapeDrawnMessage>();
            if (message != null)
            {
                messages.Add(message);
            }
        }

        return messages;
    }

    private IEnumerable<ViewportEnvelope> ReadPendingInboxEnvelopes()
    {
        if (!File.Exists(inboxPath))
        {
            inboxPosition = 0;
            yield break;
        }

        var fileInfo = new FileInfo(inboxPath);
        if (fileInfo.Length < inboxPosition)
        {
            inboxPosition = 0;
        }

        using var stream = new FileStream(inboxPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(stream);

        stream.Seek(inboxPosition, SeekOrigin.Begin);

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var envelope = JsonSerializer.Deserialize<ViewportEnvelope>(line);
            if (envelope != null)
            {
                yield return envelope;
            }
        }

        inboxPosition = stream.Position;
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
