using System.Collections.Generic;
using LaserCad.Core.Documents;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Renderuje obrys aktualnie zaznaczonych encji.
    /// </summary>
    [ExecuteAlways]
    [DefaultExecutionOrder(1000)]
    public sealed class SelectionHighlightRenderer : MonoBehaviour
    {
        [SerializeField]
        private Camera workspaceCamera;

        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private SelectionService selectionService;

        [SerializeField]
        private Material highlightMaterial;

        [SerializeField]
        private Color highlightColor = new Color(1f, 0.82f, 0.12f, 1f);

        [SerializeField]
        private Color highlightHaloColor = new Color(0f, 0f, 0f, 0.9f);

        [SerializeField]
        private Color selectionRectangleColor = new Color(1f, 0.82f, 0.12f, 0.85f);

        [SerializeField]
        private float lineWidthPixels = 3f;

        [SerializeField]
        private float haloWidthPixels = 6f;

        private void Awake()
        {
            EnsureMaterial();
        }

        private void OnRenderObject()
        {
            if (workspaceCamera == null || selectionService == null)
            {
                return;
            }

            EnsureMaterial();
            highlightMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);
            GL.Begin(GL.QUADS);

            foreach (var entity in GetSelectedEntities())
            {
                DrawBounds(entity);
            }

            if (selectionService.IsDraggingSelection)
            {
                DrawSelectionRectangle();
            }

            GL.End();
            GL.PopMatrix();
        }

        private IEnumerable<ISketchEntity> GetSelectedEntities()
        {
            if (applicationController == null || applicationController.CurrentDocument == null)
            {
                yield break;
            }

            foreach (var sketch in applicationController.CurrentDocument.Sketches)
            {
                foreach (var entity in sketch.Entities)
                {
                    if (selectionService.IsSelected(entity))
                    {
                        yield return entity;
                    }
                }
            }
        }

        private void DrawBounds(ISketchEntity entity)
        {
            var bounds = entity.Bounds;
            var min = new Vector3((float)bounds.MinX, (float)bounds.MinY, 0f);
            var max = new Vector3((float)bounds.MaxX, (float)bounds.MaxY, 0f);
            var topLeft = new Vector3(min.x, max.y, 0f);
            var bottomRight = new Vector3(max.x, min.y, 0f);

            DrawRectangleOutline(min, bottomRight, max, topLeft, highlightColor);
        }

        private void DrawSelectionRectangle()
        {
            var start = workspaceCamera.ScreenToWorldPoint(selectionService.DragStartScreenPosition);
            var current = workspaceCamera.ScreenToWorldPoint(selectionService.DragCurrentScreenPosition);
            var min = new Vector3(Mathf.Min(start.x, current.x), Mathf.Min(start.y, current.y), 0f);
            var max = new Vector3(Mathf.Max(start.x, current.x), Mathf.Max(start.y, current.y), 0f);
            var topLeft = new Vector3(min.x, max.y, 0f);
            var bottomRight = new Vector3(max.x, min.y, 0f);
            DrawRectangleOutline(min, bottomRight, max, topLeft, selectionRectangleColor);
        }

        private void DrawRectangleOutline(
            Vector3 bottomLeft,
            Vector3 bottomRight,
            Vector3 topRight,
            Vector3 topLeft,
            Color lineColor)
        {
            var worldUnitsPerPixel = GetWorldUnitsPerPixel();
            var haloWidth = worldUnitsPerPixel * haloWidthPixels;
            var lineWidth = worldUnitsPerPixel * lineWidthPixels;

            GL.Color(highlightHaloColor);
            DrawLine(bottomLeft, bottomRight, haloWidth);
            DrawLine(bottomRight, topRight, haloWidth);
            DrawLine(topRight, topLeft, haloWidth);
            DrawLine(topLeft, bottomLeft, haloWidth);

            GL.Color(lineColor);
            DrawLine(bottomLeft, bottomRight, lineWidth);
            DrawLine(bottomRight, topRight, lineWidth);
            DrawLine(topRight, topLeft, lineWidth);
            DrawLine(topLeft, bottomLeft, lineWidth);
        }

        private float GetWorldUnitsPerPixel()
        {
            if (workspaceCamera.pixelHeight <= 0)
            {
                return 1f;
            }

            return workspaceCamera.orthographicSize * 2f / workspaceCamera.pixelHeight;
        }

        private static void DrawLine(Vector3 start, Vector3 end, float width)
        {
            var direction = (end - start).normalized;
            var perpendicular = new Vector3(-direction.y, direction.x, 0f) * width * 0.5f;

            GL.Vertex(start - perpendicular);
            GL.Vertex(start + perpendicular);
            GL.Vertex(end + perpendicular);
            GL.Vertex(end - perpendicular);
        }

        private void EnsureMaterial()
        {
            if (highlightMaterial != null)
            {
                return;
            }

            var shader = Shader.Find("Hidden/Internal-Colored");
            highlightMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            highlightMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            highlightMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            highlightMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            highlightMaterial.SetInt("_ZWrite", 0);
            highlightMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
        }
    }
}
