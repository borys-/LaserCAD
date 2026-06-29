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
        private float gridExtentMillimeters = 500f;

        [SerializeField]
        private Color lineColor = new Color(0.24f, 0.24f, 0.24f, 1f);

        private void Awake()
        {
            EnsureMaterial();
        }

        private void OnValidate()
        {
            gridExtentMillimeters = Mathf.Max(1f, gridExtentMillimeters);
        }

        private void OnRenderObject()
        {
            if (!isVisible || workspaceCamera == null)
            {
                return;
            }

            EnsureMaterial();
            DrawGridBounds();
        }

        private void DrawGridBounds()
        {
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);
            GL.Begin(GL.LINES);
            GL.Color(lineColor);

            var extent = gridExtentMillimeters;
            DrawLine(new Vector3(-extent, -extent, 0f), new Vector3(extent, -extent, 0f));
            DrawLine(new Vector3(extent, -extent, 0f), new Vector3(extent, extent, 0f));
            DrawLine(new Vector3(extent, extent, 0f), new Vector3(-extent, extent, 0f));
            DrawLine(new Vector3(-extent, extent, 0f), new Vector3(-extent, -extent, 0f));

            GL.End();
            GL.PopMatrix();
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
    }
}
