using System;
using System.Collections.Generic;
using LaserCad.Core.Documents;
using LaserCad.Geometry;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Renderuje encje szkicow aktualnego dokumentu CAD w widoku 2D.
    /// </summary>
    [ExecuteAlways]
    public sealed class DocumentGeometryRenderer : MonoBehaviour
    {
        [SerializeField]
        private Camera workspaceCamera;

        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private Material lineMaterial;

        [SerializeField]
        private Color fallbackColor = new Color(0.95f, 0.95f, 0.95f, 1f);

        [SerializeField]
        private float lineWidthPixels = 2f;

        [SerializeField]
        private int circleSegments = 96;

        [SerializeField]
        private int arcSegments = 48;

        private void Awake()
        {
            EnsureMaterial();
        }

        private void OnValidate()
        {
            lineWidthPixels = Mathf.Max(1f, lineWidthPixels);
            circleSegments = Mathf.Max(12, circleSegments);
            arcSegments = Mathf.Max(4, arcSegments);
        }

        private void OnRenderObject()
        {
            if (workspaceCamera == null || applicationController == null || applicationController.CurrentDocument == null)
            {
                return;
            }

            EnsureMaterial();
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);
            GL.Begin(GL.QUADS);

            var layerColors = BuildLayerColors(applicationController.CurrentDocument);
            var width = GetWorldUnitsPerPixel() * lineWidthPixels;

            foreach (var sketch in applicationController.CurrentDocument.Sketches)
            {
                foreach (var entity in sketch.Entities)
                {
                    GL.Color(GetEntityColor(entity, layerColors));
                    DrawEntity(entity, width);
                }
            }

            GL.Color(new Color(0.15f, 0.45f, 0.95f, 1f));
            foreach (var materialSolid in applicationController.CurrentDocument.MaterialSolids)
            {
                DrawMaterialSolidFootprint(materialSolid, width);
            }

            GL.End();
            GL.PopMatrix();
        }

        private void DrawEntity(ISketchEntity entity, float width)
        {
            var line = entity as LineEntity;
            if (line != null)
            {
                DrawLine(ToVector3(line.Segment.Start), ToVector3(line.Segment.End), width);
                return;
            }

            var rectangle = entity as RectangleEntity;
            if (rectangle != null)
            {
                DrawClosedPoints(rectangle.Corners, width);
                return;
            }

            var circle = entity as CircleEntity;
            if (circle != null)
            {
                DrawCircle(circle.Circle, width);
                return;
            }

            var arc = entity as ArcEntity;
            if (arc != null)
            {
                DrawArc(arc.Arc, width);
                return;
            }

            var polyline = entity as PolylineEntity;
            if (polyline != null)
            {
                DrawPolyline(polyline.Polyline, width);
                return;
            }

            var text = entity as TextEntity;
            if (text != null)
            {
                DrawTextPlaceholder(text, width);
            }
        }

        private void DrawCircle(Circle2D circle, float width)
        {
            var previous = PointOnCircle(circle, 0.0);

            for (var index = 1; index <= circleSegments; index++)
            {
                var current = PointOnCircle(circle, (Math.PI * 2.0) * index / circleSegments);
                DrawLine(previous, current, width);
                previous = current;
            }
        }

        private void DrawArc(Arc2D arc, float width)
        {
            var previous = ToVector3(arc.PointAt(0.0));

            for (var index = 1; index <= arcSegments; index++)
            {
                var current = ToVector3(arc.PointAt(index / (double)arcSegments));
                DrawLine(previous, current, width);
                previous = current;
            }
        }

        private void DrawPolyline(Polyline2D polyline, float width)
        {
            for (var index = 1; index < polyline.Points.Count; index++)
            {
                DrawLine(ToVector3(polyline.Points[index - 1]), ToVector3(polyline.Points[index]), width);
            }

            if (polyline.IsClosed && polyline.Points.Count > 1)
            {
                DrawLine(ToVector3(polyline.Points[polyline.Points.Count - 1]), ToVector3(polyline.Points[0]), width);
            }
        }

        private void DrawClosedPoints(IReadOnlyList<Point2D> points, float width)
        {
            for (var index = 1; index < points.Count; index++)
            {
                DrawLine(ToVector3(points[index - 1]), ToVector3(points[index]), width);
            }

            if (points.Count > 1)
            {
                DrawLine(ToVector3(points[points.Count - 1]), ToVector3(points[0]), width);
            }
        }

        private void DrawTextPlaceholder(TextEntity text, float width)
        {
            var position = ToVector3(text.Position);
            var size = (float)text.Height;

            DrawLine(position, position + new Vector3(size, 0f, 0f), width);
            DrawLine(position, position + new Vector3(0f, size, 0f), width);
        }

        private void DrawMaterialSolidFootprint(LaserCad.Core.MaterialModel.MaterialSolid materialSolid, float width)
        {
            var bounds = materialSolid.Mesh.Bounds2D;
            var offset = materialSolid.Orientation.Position;
            var a = new Vector3((float)(bounds.MinX + offset.X), (float)(bounds.MinY + offset.Y), 0f);
            var b = new Vector3((float)(bounds.MaxX + offset.X), (float)(bounds.MinY + offset.Y), 0f);
            var c = new Vector3((float)(bounds.MaxX + offset.X), (float)(bounds.MaxY + offset.Y), 0f);
            var d = new Vector3((float)(bounds.MinX + offset.X), (float)(bounds.MaxY + offset.Y), 0f);

            DrawLine(a, b, width);
            DrawLine(b, c, width);
            DrawLine(c, d, width);
            DrawLine(d, a, width);
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

        private static Vector3 ToVector3(Point2D point)
        {
            return new Vector3((float)point.X, (float)point.Y, 0f);
        }

        private static Vector3 PointOnCircle(Circle2D circle, double angleRadians)
        {
            return new Vector3(
                (float)(circle.Center.X + (Math.Cos(angleRadians) * circle.Radius)),
                (float)(circle.Center.Y + (Math.Sin(angleRadians) * circle.Radius)),
                0f);
        }

        private Color GetEntityColor(ISketchEntity entity, IReadOnlyDictionary<string, Color> layerColors)
        {
            Color color;
            return entity != null && layerColors.TryGetValue(entity.LayerName, out color)
                ? color
                : fallbackColor;
        }

        private static Dictionary<string, Color> BuildLayerColors(CadDocument document)
        {
            var colors = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);

            foreach (var layer in document.Layers)
            {
                Color color;
                if (TryParseLayerColor(layer.Color.Hex, out color))
                {
                    colors[layer.Name] = color;
                }
            }

            return colors;
        }

        private static bool TryParseLayerColor(string value, out Color color)
        {
            color = Color.white;

            if (string.IsNullOrEmpty(value) || value.Length != 7 || value[0] != '#')
            {
                return false;
            }

            byte red;
            byte green;
            byte blue;
            if (!byte.TryParse(value.Substring(1, 2), System.Globalization.NumberStyles.HexNumber, null, out red)
                || !byte.TryParse(value.Substring(3, 2), System.Globalization.NumberStyles.HexNumber, null, out green)
                || !byte.TryParse(value.Substring(5, 2), System.Globalization.NumberStyles.HexNumber, null, out blue))
            {
                return false;
            }

            color = new Color(red / 255f, green / 255f, blue / 255f, 1f);
            return true;
        }

        private void EnsureMaterial()
        {
            if (lineMaterial != null)
            {
                return;
            }

            var shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }
}
