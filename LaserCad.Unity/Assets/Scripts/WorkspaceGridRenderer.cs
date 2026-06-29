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

        private void Awake()
        {
            EnsureMaterial();
        }

        private void OnValidate()
        {
            minorStepMillimeters = Mathf.Max(0.1f, minorStepMillimeters);
            mediumStepMillimeters = Mathf.Max(minorStepMillimeters, mediumStepMillimeters);
            majorStepMillimeters = Mathf.Max(mediumStepMillimeters, majorStepMillimeters);
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
            GL.Begin(GL.LINES);
            GL.Color(minorLineColor);

            DrawVerticalLines(bounds, minorStepMillimeters);
            DrawHorizontalLines(bounds, minorStepMillimeters);

            GL.Color(mediumLineColor);
            DrawVerticalLines(bounds, mediumStepMillimeters);
            DrawHorizontalLines(bounds, mediumStepMillimeters);

            GL.Color(majorLineColor);
            DrawVerticalLines(bounds, majorStepMillimeters);
            DrawHorizontalLines(bounds, majorStepMillimeters);

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

        private static void DrawVerticalLines(Bounds2D bounds, float step)
        {
            var start = Mathf.Floor(bounds.MinimumX / step) * step;
            var end = Mathf.Ceil(bounds.MaximumX / step) * step;

            for (var x = start; x <= end; x += step)
            {
                DrawLine(new Vector3(x, bounds.MinimumY, 0f), new Vector3(x, bounds.MaximumY, 0f));
            }
        }

        private static void DrawHorizontalLines(Bounds2D bounds, float step)
        {
            var start = Mathf.Floor(bounds.MinimumY / step) * step;
            var end = Mathf.Ceil(bounds.MaximumY / step) * step;

            for (var y = start; y <= end; y += step)
            {
                DrawLine(new Vector3(bounds.MinimumX, y, 0f), new Vector3(bounds.MaximumX, y, 0f));
            }
        }

        private static void DrawLine(Vector3 start, Vector3 end)
        {
            GL.Vertex(start);
            GL.Vertex(end);
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
