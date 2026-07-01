using System;
using System.Collections.Generic;
using LaserCad.Core.Documents;
using LaserCad.Core.Preview3D;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Renderuje uproszczony podglad 3D aktualnego dokumentu CAD.
    /// </summary>
    [ExecuteAlways]
    public sealed class ThreeDPreviewRenderer : MonoBehaviour
    {
        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private bool previewVisible = true;

        [SerializeField]
        private bool animateFolding = true;

        [SerializeField]
        private float animationSeconds = 4f;

        [SerializeField]
        private Color plywoodColor = new Color(0.78f, 0.58f, 0.32f, 1f);

        [SerializeField]
        private Color collisionColor = new Color(0.95f, 0.25f, 0.18f, 1f);

        [SerializeField]
        private Vector3 previewOrigin = new Vector3(170f, 30f, 0f);

        private readonly Contour3DBuilder builder = new Contour3DBuilder();
        private readonly List<Renderer> partRenderers = new List<Renderer>();
        private readonly List<Transform> partTransforms = new List<Transform>();
        private CadDocument renderedDocument;
        private GameObject previewRoot;
        private Material plywoodMaterial;
        private Material collisionMaterial;

        private void OnEnable()
        {
            RebuildIfNeeded(true);
        }

        private void OnDisable()
        {
            ClearPreview();
        }

        private void Update()
        {
            RebuildIfNeeded(false);
            UpdateVisibility();
            UpdateAssemblyPose();
            UpdateCollisionMaterials();
        }

        /// <summary>
        /// Wlacza albo wylacza podglad 3D.
        /// </summary>
        public void SetPreviewVisible(bool isVisible)
        {
            previewVisible = isVisible;
            UpdateVisibility();
        }

        private void RebuildIfNeeded(bool force)
        {
            if (applicationController == null || applicationController.CurrentDocument == null)
            {
                ClearPreview();
                return;
            }

            if (!force && ReferenceEquals(renderedDocument, applicationController.CurrentDocument))
            {
                return;
            }

            renderedDocument = applicationController.CurrentDocument;
            RebuildPreview(renderedDocument);
        }

        private void RebuildPreview(CadDocument document)
        {
            ClearPreview();
            EnsureMaterials();

            previewRoot = new GameObject("3D Preview Root");
            previewRoot.hideFlags = HideFlags.DontSave;
            previewRoot.transform.SetParent(transform, false);
            previewRoot.transform.localPosition = previewOrigin;
            previewRoot.transform.localRotation = Quaternion.Euler(55f, 0f, -35f);

            var thickness = document.MaterialProfile != null
                ? document.MaterialProfile.Thickness.Millimeters
                : 3.0;

            var partIndex = 0;
            foreach (var sketch in document.Sketches)
            {
                foreach (var entity in sketch.Entities)
                {
                    var part = TryCreatePart(entity, thickness, "Part " + (partIndex + 1));
                    if (part == null)
                    {
                        continue;
                    }

                    CreatePartObject(part, partIndex);
                    partIndex++;
                }
            }
        }

        private Part3D TryCreatePart(ISketchEntity entity, double thickness, string name)
        {
            try
            {
                var rectangle = entity as RectangleEntity;
                if (rectangle != null)
                {
                    return builder.FromRectangle(rectangle, thickness, name);
                }

                var polyline = entity as PolylineEntity;
                if (polyline != null && polyline.Polyline.IsClosed)
                {
                    return builder.FromPolyline(polyline, thickness, name);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning("Nie udalo sie zbudowac czesci 3D: " + exception.Message);
            }

            return null;
        }

        private void CreatePartObject(Part3D part, int partIndex)
        {
            var mesh = new Mesh
            {
                name = part.Name
            };

            var vertices = new Vector3[part.Mesh.Vertices.Count];
            for (var index = 0; index < vertices.Length; index++)
            {
                var vertex = part.Mesh.Vertices[index];
                vertices[index] = new Vector3((float)vertex.X, (float)vertex.Y, (float)vertex.Z);
            }

            var triangles = new int[part.Mesh.TriangleIndices.Count];
            for (var index = 0; index < triangles.Length; index++)
            {
                triangles[index] = part.Mesh.TriangleIndices[index];
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            var partObject = new GameObject(part.Name);
            partObject.hideFlags = HideFlags.DontSave;
            partObject.transform.SetParent(previewRoot.transform, false);
            partObject.transform.localPosition = GetFlatPartOffset(partIndex);

            var meshFilter = partObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            var meshRenderer = partObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = plywoodMaterial;

            partTransforms.Add(partObject.transform);
            partRenderers.Add(meshRenderer);
        }

        private void UpdateAssemblyPose()
        {
            if (previewRoot == null)
            {
                return;
            }

            var fold = animateFolding && Application.isPlaying
                ? Mathf.PingPong(Time.time / Mathf.Max(0.1f, animationSeconds), 1f)
                : 0.55f;

            for (var index = 0; index < partTransforms.Count; index++)
            {
                var partTransform = partTransforms[index];
                var angle = index == 0 ? 0f : Mathf.Lerp(0f, 90f, fold);
                partTransform.localPosition = Vector3.Lerp(GetFlatPartOffset(index), GetFoldedPartOffset(index), fold);
                partTransform.localRotation = Quaternion.Euler(index == 0 ? 0f : angle, 0f, 0f);
            }
        }

        private void UpdateCollisionMaterials()
        {
            for (var index = 0; index < partRenderers.Count; index++)
            {
                partRenderers[index].sharedMaterial = HasCollision(index) ? collisionMaterial : plywoodMaterial;
            }
        }

        private bool HasCollision(int partIndex)
        {
            if (partIndex < 0 || partIndex >= partRenderers.Count)
            {
                return false;
            }

            var bounds = partRenderers[partIndex].bounds;
            for (var otherIndex = 0; otherIndex < partRenderers.Count; otherIndex++)
            {
                if (otherIndex == partIndex)
                {
                    continue;
                }

                if (bounds.Intersects(partRenderers[otherIndex].bounds))
                {
                    return true;
                }
            }

            return false;
        }

        private static Vector3 GetFlatPartOffset(int index)
        {
            return new Vector3((index % 3) * 75f, -(index / 3) * 55f, 0f);
        }

        private static Vector3 GetFoldedPartOffset(int index)
        {
            if (index == 0)
            {
                return Vector3.zero;
            }

            var side = (index - 1) % 4;
            if (side == 0)
            {
                return new Vector3(0f, 42f, 0f);
            }

            if (side == 1)
            {
                return new Vector3(42f, 0f, 0f);
            }

            if (side == 2)
            {
                return new Vector3(0f, -42f, 0f);
            }

            return new Vector3(-42f, 0f, 0f);
        }

        private void UpdateVisibility()
        {
            if (previewRoot != null)
            {
                previewRoot.SetActive(previewVisible);
            }
        }

        private void EnsureMaterials()
        {
            if (plywoodMaterial == null)
            {
                plywoodMaterial = CreateMaterial("LaserCad Plywood Preview", plywoodColor);
            }

            if (collisionMaterial == null)
            {
                collisionMaterial = CreateMaterial("LaserCad Collision Preview", collisionColor);
            }
        }

        private static Material CreateMaterial(string name, Color color)
        {
            var shader = Shader.Find("Standard");
            var material = new Material(shader)
            {
                name = name,
                hideFlags = HideFlags.DontSave
            };

            material.color = color;
            return material;
        }

        private void ClearPreview()
        {
            partRenderers.Clear();
            partTransforms.Clear();

            if (previewRoot == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(previewRoot);
            }
            else
            {
                DestroyImmediate(previewRoot);
            }

            previewRoot = null;
        }
    }
}
