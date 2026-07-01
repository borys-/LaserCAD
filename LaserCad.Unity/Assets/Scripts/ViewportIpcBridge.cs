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

        [SerializeField]
        private Camera workspaceCamera;

        [SerializeField]
        private Material previewMaterial;

        [SerializeField]
        private Color previewColor = new Color(1f, 0.82f, 0.12f, 0.9f);

        [SerializeField]
        private float previewLineWidthPixels = 2f;

        [SerializeField]
        private float dragDrawThresholdPixels = 4f;

        private readonly DocumentSerializer documentSerializer = new DocumentSerializer();
        private readonly HashSet<Guid> lastSentSelection = new HashSet<Guid>();
        private string outboxPath;
        private string inboxPath;
        private long outboxPosition;
        private ViewportDrawingTool activeDrawingTool = ViewportDrawingTool.None;
        private bool hasDrawingStartPoint;
        private bool isDrawingPointerDown;
        private bool isCompletingClickDraw;
        private ViewportPoint drawingStartPoint;
        private Vector2 drawingStartScreenPosition;
        private ViewportShapeDrawnMessage pendingShapePreview;

        private void Awake()
        {
            if (ViewportProcessMode.IsViewportProcess())
            {
                Application.runInBackground = true;
            }

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

        private void OnDisable()
        {
            Cursor.visible = true;
        }

        private void OnApplicationQuit()
        {
            Cursor.visible = true;
        }

        private void Update()
        {
            if (!ViewportProcessMode.IsViewportProcess())
            {
                return;
            }

            ReadPendingOutboxMessages();
            HandleDrawingInput();
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
                    return;
                }

                if (envelope.Kind == ViewportMessageKind.DrawingToolChanged)
                {
                    HandleDrawingToolChanged(envelope.Payload.Deserialize<ViewportDrawingToolChangedMessage>());
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Nie udalo sie obsluzyc komunikatu IPC viewportu: " + exception.Message);
            }
        }

        private void HandleDrawingToolChanged(ViewportDrawingToolChangedMessage message)
        {
            if (message == null)
            {
                return;
            }

            activeDrawingTool = message.Tool;
            hasDrawingStartPoint = false;
            isDrawingPointerDown = false;
            isCompletingClickDraw = false;
            UpdateCursorVisibility();

            if (selectionService != null)
            {
                selectionService.SetInputEnabled(activeDrawingTool == ViewportDrawingTool.None);
            }
        }

        private void UpdateCursorVisibility()
        {
            Cursor.visible = activeDrawingTool == ViewportDrawingTool.None;
        }

        private void HandleDrawingInput()
        {
            if (activeDrawingTool == ViewportDrawingTool.None || workspaceCamera == null)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                isDrawingPointerDown = true;
                isCompletingClickDraw = hasDrawingStartPoint;
                drawingStartScreenPosition = Input.mousePosition;

                if (!hasDrawingStartPoint)
                {
                    drawingStartPoint = GetMouseWorldPoint();
                    hasDrawingStartPoint = true;
                }

                return;
            }

            if (!isDrawingPointerDown || !Input.GetMouseButtonUp(0))
            {
                return;
            }

            isDrawingPointerDown = false;

            var endPoint = GetMouseWorldPoint();
            var dragDistance = Vector2.Distance(drawingStartScreenPosition, Input.mousePosition);
            if (dragDistance < dragDrawThresholdPixels && !isCompletingClickDraw)
            {
                return;
            }

            PublishDrawnShape(endPoint);
        }

        private void PublishDrawnShape(ViewportPoint endPoint)
        {
            if (Math.Abs(drawingStartPoint.X - endPoint.X) <= double.Epsilon
                && Math.Abs(drawingStartPoint.Y - endPoint.Y) <= double.Epsilon)
            {
                return;
            }

            pendingShapePreview = new ViewportShapeDrawnMessage(activeDrawingTool, drawingStartPoint, endPoint);
            AppendInboxMessage(
                ViewportMessageKind.ShapeDrawn,
                pendingShapePreview);
            hasDrawingStartPoint = false;
        }

        private ViewportPoint GetMouseWorldPoint()
        {
            var world = workspaceCamera.ScreenToWorldPoint(Input.mousePosition);
            return new ViewportPoint(world.x, world.y);
        }

        private void HandleDocumentSnapshot(ViewportDocumentSnapshot snapshot)
        {
            if (snapshot == null || applicationController == null)
            {
                return;
            }

            var document = documentSerializer.Deserialize(snapshot.DocumentJson);
            applicationController.LoadDocument(document);
            pendingShapePreview = null;

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

        private void OnRenderObject()
        {
            if (!ViewportProcessMode.IsViewportProcess() || workspaceCamera == null)
            {
                return;
            }

            var preview = GetCurrentPreview();
            if (preview == null)
            {
                return;
            }

            EnsurePreviewMaterial();
            previewMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);
            GL.Begin(GL.QUADS);
            GL.Color(previewColor);
            DrawPreview(preview);
            GL.End();
            GL.PopMatrix();
        }

        private ViewportShapeDrawnMessage GetCurrentPreview()
        {
            if (hasDrawingStartPoint && activeDrawingTool != ViewportDrawingTool.None)
            {
                return new ViewportShapeDrawnMessage(
                    activeDrawingTool,
                    drawingStartPoint,
                    GetMouseWorldPoint());
            }

            return pendingShapePreview;
        }

        private void DrawPreview(ViewportShapeDrawnMessage preview)
        {
            var start = ToVector3(preview.Start);
            var end = ToVector3(preview.End);
            var width = GetWorldUnitsPerPixel() * previewLineWidthPixels;

            if (preview.Tool == ViewportDrawingTool.Line)
            {
                DrawLine(start, end, width);
                return;
            }

            if (preview.Tool == ViewportDrawingTool.Rectangle)
            {
                DrawRectangle(start, end, width);
                return;
            }

            if (preview.Tool == ViewportDrawingTool.MaterialPlate)
            {
                DrawRectangle(start, end, width);
                return;
            }

            if (preview.Tool == ViewportDrawingTool.Circle)
            {
                DrawCircle(start, Vector3.Distance(start, end), width);
            }
        }

        private void DrawRectangle(Vector3 start, Vector3 end, float width)
        {
            var a = new Vector3(start.x, start.y, 0f);
            var b = new Vector3(end.x, start.y, 0f);
            var c = new Vector3(end.x, end.y, 0f);
            var d = new Vector3(start.x, end.y, 0f);

            DrawLine(a, b, width);
            DrawLine(b, c, width);
            DrawLine(c, d, width);
            DrawLine(d, a, width);
        }

        private void DrawCircle(Vector3 center, float radius, float width)
        {
            const int SegmentCount = 96;
            if (radius <= Mathf.Epsilon)
            {
                return;
            }

            var previous = PointOnCircle(center, radius, 0f);
            for (var index = 1; index <= SegmentCount; index++)
            {
                var angle = Mathf.PI * 2f * index / SegmentCount;
                var current = PointOnCircle(center, radius, angle);
                DrawLine(previous, current, width);
                previous = current;
            }
        }

        private float GetWorldUnitsPerPixel()
        {
            if (workspaceCamera.pixelHeight <= 0)
            {
                return 1f;
            }

            return workspaceCamera.orthographicSize * 2f / workspaceCamera.pixelHeight;
        }

        private static Vector3 ToVector3(ViewportPoint point)
        {
            return new Vector3((float)point.X, (float)point.Y, 0f);
        }

        private static Vector3 PointOnCircle(Vector3 center, float radius, float angleRadians)
        {
            return new Vector3(
                center.x + Mathf.Cos(angleRadians) * radius,
                center.y + Mathf.Sin(angleRadians) * radius,
                0f);
        }

        private static void DrawLine(Vector3 start, Vector3 end, float width)
        {
            var delta = end - start;
            if (delta.sqrMagnitude <= Mathf.Epsilon)
            {
                return;
            }

            var direction = delta.normalized;
            var perpendicular = new Vector3(-direction.y, direction.x, 0f) * width * 0.5f;

            GL.Vertex(start - perpendicular);
            GL.Vertex(start + perpendicular);
            GL.Vertex(end + perpendicular);
            GL.Vertex(end - perpendicular);
        }

        private void EnsurePreviewMaterial()
        {
            if (previewMaterial != null)
            {
                return;
            }

            var shader = Shader.Find("Hidden/Internal-Colored");
            previewMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            previewMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            previewMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            previewMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            previewMaterial.SetInt("_ZWrite", 0);
        }
    }
}
