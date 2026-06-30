using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using LaserCad.Core.Documents;
using LaserCad.ViewportContract;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Obsluguje prosty transport IPC JSON lines miedzy desktop shell i viewportem Unity.
    /// </summary>
    public sealed class ViewportIpcBridge : MonoBehaviour
    {
        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private SelectionService selectionService;

        [SerializeField]
        private WorkspaceCameraController cameraController;

        [SerializeField]
        private WorkspaceGridRenderer gridRenderer;

        private readonly DocumentSerializer documentSerializer = new DocumentSerializer();
        private readonly HashSet<Guid> lastSentSelection = new HashSet<Guid>();
        private string outboxPath;
        private string inboxPath;
        private long outboxPosition;

        private void Awake()
        {
            var ipcDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LaserCad");

            outboxPath = Path.Combine(ipcDirectory, "viewport-outbox.jsonl");
            inboxPath = Path.Combine(ipcDirectory, "viewport-inbox.jsonl");

            Directory.CreateDirectory(ipcDirectory);

            if (File.Exists(outboxPath))
            {
                outboxPosition = new FileInfo(outboxPath).Length;
            }
        }

        private void Update()
        {
            if (!ViewportProcessMode.IsViewportProcess())
            {
                return;
            }

            ReadPendingOutboxMessages();
            SendSelectionIfChanged();
        }

        private void ReadPendingOutboxMessages()
        {
            if (!File.Exists(outboxPath))
            {
                outboxPosition = 0;
                return;
            }

            var fileInfo = new FileInfo(outboxPath);
            if (fileInfo.Length < outboxPosition)
            {
                outboxPosition = 0;
            }

            using (var stream = new FileStream(outboxPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = new StreamReader(stream))
            {
                stream.Seek(outboxPosition, SeekOrigin.Begin);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        HandleOutboxLine(line);
                    }
                }

                outboxPosition = stream.Position;
            }
        }

        private void HandleOutboxLine(string line)
        {
            try
            {
                var envelope = JsonSerializer.Deserialize<ViewportEnvelope>(line);
                if (envelope == null)
                {
                    return;
                }

                if (envelope.Kind == ViewportMessageKind.DocumentSnapshot)
                {
                    HandleDocumentSnapshot(envelope.Payload.Deserialize<ViewportDocumentSnapshot>());
                    return;
                }

                if (envelope.Kind == ViewportMessageKind.ViewCommand)
                {
                    HandleViewCommand(envelope.Payload.Deserialize<ViewportViewCommandMessage>());
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Nie udalo sie obsluzyc komunikatu IPC viewportu: " + exception.Message);
            }
        }

        private void HandleDocumentSnapshot(ViewportDocumentSnapshot snapshot)
        {
            if (snapshot == null || applicationController == null)
            {
                return;
            }

            var document = documentSerializer.Deserialize(snapshot.DocumentJson);
            applicationController.LoadDocument(document);

            if (selectionService != null)
            {
                selectionService.Clear();
            }
        }

        private void HandleViewCommand(ViewportViewCommandMessage message)
        {
            if (message == null)
            {
                return;
            }

            if (message.Command == ViewportViewCommand.ResetView && cameraController != null)
            {
                cameraController.ResetView();
                return;
            }

            if (message.Command == ViewportViewCommand.ZoomToFit && cameraController != null)
            {
                cameraController.ResetView();
                return;
            }

            if (message.Command == ViewportViewCommand.SetGridVisibility && gridRenderer != null && message.Enabled.HasValue)
            {
                gridRenderer.SetVisible(message.Enabled.Value);
            }
        }

        private void SendSelectionIfChanged()
        {
            if (selectionService == null)
            {
                return;
            }

            var currentSelection = new HashSet<Guid>(selectionService.SelectedEntityIds);
            if (currentSelection.SetEquals(lastSentSelection))
            {
                return;
            }

            lastSentSelection.Clear();
            foreach (var entityId in currentSelection)
            {
                lastSentSelection.Add(entityId);
            }

            var message = new ViewportSelectionChangedMessage(currentSelection.ToArray());
            AppendInboxMessage(ViewportMessageKind.SelectionChanged, message);
        }

        private void AppendInboxMessage<TPayload>(ViewportMessageKind kind, TPayload payload)
        {
            var payloadElement = JsonSerializer.SerializeToElement(payload);
            var envelope = new ViewportEnvelope(kind, payloadElement);
            File.AppendAllText(inboxPath, JsonSerializer.Serialize(envelope) + Environment.NewLine);
        }
    }
}
