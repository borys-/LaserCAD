using System.Collections.Generic;
using LaserCad.Core.Documents;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Renderuje marker aktywnego punktu snapowania.
    /// </summary>
    [ExecuteAlways]
    public sealed class SnapMarkerRenderer : MonoBehaviour
    {
        [SerializeField]
        private Camera workspaceCamera;

        [SerializeField]
        private SnapService snapService;

        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private Material markerMaterial;

        [SerializeField]
        private Color markerColor = new Color(1f, 0.78f, 0.2f, 1f);

        [SerializeField]
        private float markerSizePixels = 12f;

        [SerializeField]
        private float markerLineWidthPixels = 2f;

        private SnapResult currentSnap;

        private void Awake()
        {
            EnsureMaterial();
        }

        private void Update()
        {
            UpdateSnapMarker();
        }

        private void OnRenderObject()
        {
            if (!currentSnap.HasSnap || workspaceCamera == null)
            {
                return;
            }

            EnsureMaterial();
            DrawMarker();
        }

        private void UpdateSnapMarker()
        {
            if (workspaceCamera == null || snapService == null)
            {
                currentSnap = new SnapResult(Vector2.zero, false, SnapPriority.Grid);
                return;
            }

            var world = workspaceCamera.ScreenToWorldPoint(Input.mousePosition);
            currentSnap = snapService.Snap(new Vector2(world.x, world.y), GetEntities());
        }

        private IEnumerable<ISketchEntity> GetEntities()
        {
            if (applicationController == null || applicationController.CurrentDocument == null)
            {
                yield break;
            }

            foreach (var sketch in applicationController.CurrentDocument.Sketches)
            {
                foreach (var entity in sketch.Entities)
                {
                    yield return entity;
                }
            }
        }

        private void DrawMarker()
        {
            markerMaterial.SetPass(0);

            var worldUnitsPerPixel = GetWorldUnitsPerPixel();
            var size = markerSizePixels * worldUnitsPerPixel;
            var width = markerLineWidthPixels * worldUnitsPerPixel;
            var center = new Vector3(currentSnap.Position.x, currentSnap.Position.y, 0f);

            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.identity);
            GL.Begin(GL.QUADS);
            GL.Color(markerColor);
            DrawLine(center + Vector3.left * size, center + Vector3.right * size, width);
            DrawLine(center + Vector3.down * size, center + Vector3.up * size, width);
            GL.End();
            GL.PopMatrix();
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
            if (markerMaterial != null)
            {
                return;
            }

            var shader = Shader.Find("Hidden/Internal-Colored");
            markerMaterial = new Material(shader)
            {
                hideFlags = HideFlags.HideAndDontSave
            };

            markerMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            markerMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            markerMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            markerMaterial.SetInt("_ZWrite", 0);
        }
    }
}
