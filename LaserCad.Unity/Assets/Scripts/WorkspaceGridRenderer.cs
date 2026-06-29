using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Renderuje pomocnicza siatke obszaru roboczego 2D.
    /// </summary>
    [ExecuteAlways]
    public sealed class WorkspaceGridRenderer : MonoBehaviour
    {
        [SerializeField]
        private Camera workspaceCamera;

        [SerializeField]
        private Material lineMaterial;

        [SerializeField]
        private bool isVisible = true;

        [SerializeField]
        private float minorStepMillimeters = 1f;

        [SerializeField]
        private float mediumStepMillimeters = 5f;

        [SerializeField]
        private float majorStepMillimeters = 10f;

        [SerializeField]
        private Color minorLineColor = new Color(0.20f, 0.20f, 0.20f, 1f);

        [SerializeField]
        private Color mediumLineColor = new Color(0.31f, 0.31f, 0.31f, 1f);

        [SerializeField]
        private Color majorLineColor = new Color(0.44f, 0.44f, 0.44f, 1f);

        [SerializeField]
        private float minorLineWidthPixels = 1f;

        [SerializeField]
        private float mediumLineWidthPixels = 1.5f;

        [SerializeField]
        private float majorLineWidthPixels = 2f;

        private void Awake()
        {
            EnsureMaterial();
        }

        private void OnValidate()
        {
            minorStepMillimeters = Mathf.Max(0.1f, minorStepMillimeters);
            mediumStepMillimeters = Mathf.Max(minorStepMillimeters, mediumStepMillimeters);
            majorStepMillimeters = Mathf.Max(mediumStepMillimeters, majorStepMillimeters);
            minorLineWidthPixels = Mathf.Max(1f, minorLineWidthPixels);
            mediumLineWidthPixels = Mathf.Max(minorLineWidthPixels, mediumLineWidthPixels);
            majorLineWidthPixels = Mathf.Max(mediumLineWidthPixels, majorLineWidthPixels);
        }

        private void OnRenderObject()
        {
            if (!isVisible || workspaceCamera == null)
            {
                return;
            }

            EnsureMaterial();
            DrawGrid();
        }

        private void DrawGrid()
        {
            var bounds = GetVisibleBounds();

            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);
            GL.Begin(GL.QUADS);
            GL.Color(minorLineColor);

            var worldUnitsPerPixel = GetWorldUnitsPerPixel();

            DrawVerticalLines(bounds, minorStepMillimeters, minorLineWidthPixels * worldUnitsPerPixel);
            DrawHorizontalLines(bounds, minorStepMillimeters, minorLineWidthPixels * worldUnitsPerPixel);

            GL.Color(mediumLineColor);
            DrawVerticalLines(bounds, mediumStepMillimeters, mediumLineWidthPixels * worldUnitsPerPixel);
            DrawHorizontalLines(bounds, mediumStepMillimeters, mediumLineWidthPixels * worldUnitsPerPixel);

            GL.Color(majorLineColor);
            DrawVerticalLines(bounds, majorStepMillimeters, majorLineWidthPixels * worldUnitsPerPixel);
            DrawHorizontalLines(bounds, majorStepMillimeters, majorLineWidthPixels * worldUnitsPerPixel);

            GL.End();
            GL.PopMatrix();
        }

        private Bounds2D GetVisibleBounds()
        {
            var min = workspaceCamera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f));
            var max = workspaceCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));

            return new Bounds2D(
                Mathf.Min(min.x, max.x),
                Mathf.Max(min.x, max.x),
                Mathf.Min(min.y, max.y),
                Mathf.Max(min.y, max.y));
        }

        private float GetWorldUnitsPerPixel()
        {
            if (workspaceCamera.pixelHeight <= 0)
            {
                return 1f;
            }

            return workspaceCamera.orthographicSize * 2f / workspaceCamera.pixelHeight;
        }

        private static void DrawVerticalLines(Bounds2D bounds, float step, float width)
        {
            var start = Mathf.Floor(bounds.MinimumX / step) * step;
            var end = Mathf.Ceil(bounds.MaximumX / step) * step;

            for (var x = start; x <= end; x += step)
            {
                DrawLine(new Vector3(x, bounds.MinimumY, 0f), new Vector3(x, bounds.MaximumY, 0f), width);
            }
        }

        private static void DrawHorizontalLines(Bounds2D bounds, float step, float width)
        {
            var start = Mathf.Floor(bounds.MinimumY / step) * step;
            var end = Mathf.Ceil(bounds.MaximumY / step) * step;

            for (var y = start; y <= end; y += step)
            {
                DrawLine(new Vector3(bounds.MinimumX, y, 0f), new Vector3(bounds.MaximumX, y, 0f), width);
            }
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

        private readonly struct Bounds2D
        {
            public Bounds2D(float minimumX, float maximumX, float minimumY, float maximumY)
            {
                MinimumX = minimumX;
                MaximumX = maximumX;
                MinimumY = minimumY;
                MaximumY = maximumY;
            }

            public float MinimumX { get; }

            public float MaximumX { get; }

            public float MinimumY { get; }

            public float MaximumY { get; }
        }
    }
}
