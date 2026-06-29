using System;
using System.Collections.Generic;
using LaserCad.Core.Documents;
using LaserCad.Geometry;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Przechowuje aktualne zaznaczenie encji szkicu w widoku Unity.
    /// </summary>
    public sealed class SelectionService : MonoBehaviour
    {
        [SerializeField]
        private Camera workspaceCamera;

        [SerializeField]
        private LaserCadApplicationController applicationController;

        [SerializeField]
        private float clickToleranceMillimeters = 2f;

        [SerializeField]
        private float dragThresholdPixels = 4f;

        private readonly HashSet<Guid> selectedEntityIds = new HashSet<Guid>();
        private Vector2 mouseDownScreenPosition;
        private bool isPointerDown;

        /// <summary>
        /// Identyfikatory aktualnie zaznaczonych encji.
        /// </summary>
        public IReadOnlyCollection<Guid> SelectedEntityIds
        {
            get { return selectedEntityIds; }
        }

        /// <summary>
        /// Liczba zaznaczonych encji.
        /// </summary>
        public int SelectionCount
        {
            get { return selectedEntityIds.Count; }
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isPointerDown = true;
                mouseDownScreenPosition = Input.mousePosition;
            }

            if (isPointerDown && Input.GetMouseButtonUp(0))
            {
                FinishPointerSelection();
                isPointerDown = false;
            }
        }

        /// <summary>
        /// Czysci zaznaczenie.
        /// </summary>
        public void Clear()
        {
            selectedEntityIds.Clear();
        }

        /// <summary>
        /// Zaznacza tylko jedna encje.
        /// </summary>
        public void SelectOnly(ISketchEntity entity)
        {
            selectedEntityIds.Clear();

            if (entity != null)
            {
                selectedEntityIds.Add(entity.Id);
            }
        }

        /// <summary>
        /// Przelacza zaznaczenie encji bez zmiany pozostalych elementow.
        /// </summary>
        public void Toggle(ISketchEntity entity)
        {
            if (entity == null)
            {
                return;
            }

            if (!selectedEntityIds.Remove(entity.Id))
            {
                selectedEntityIds.Add(entity.Id);
            }
        }

        /// <summary>
        /// Sprawdza, czy encja jest zaznaczona.
        /// </summary>
        public bool IsSelected(ISketchEntity entity)
        {
            return entity != null && selectedEntityIds.Contains(entity.Id);
        }

        private void FinishPointerSelection()
        {
            var currentScreenPosition = (Vector2)Input.mousePosition;
            if (Vector2.Distance(mouseDownScreenPosition, currentScreenPosition) < dragThresholdPixels)
            {
                SelectAtMousePosition();
                return;
            }

            SelectByRectangle(mouseDownScreenPosition, currentScreenPosition);
        }

        private void SelectAtMousePosition()
        {
            var entity = FindEntityAtMousePosition();
            var isMultiSelect = IsMultiSelectModifierPressed();

            if (entity != null)
            {
                if (isMultiSelect)
                {
                    Toggle(entity);
                }
                else
                {
                    SelectOnly(entity);
                }

                return;
            }

            if (!isMultiSelect)
            {
                Clear();
            }
        }

        private void SelectByRectangle(Vector2 startScreenPosition, Vector2 endScreenPosition)
        {
            if (workspaceCamera == null)
            {
                return;
            }

            var startWorld = workspaceCamera.ScreenToWorldPoint(startScreenPosition);
            var endWorld = workspaceCamera.ScreenToWorldPoint(endScreenPosition);
            var minX = Math.Min(startWorld.x, endWorld.x);
            var maxX = Math.Max(startWorld.x, endWorld.x);
            var minY = Math.Min(startWorld.y, endWorld.y);
            var maxY = Math.Max(startWorld.y, endWorld.y);
            var isMultiSelect = IsMultiSelectModifierPressed();

            if (!isMultiSelect)
            {
                Clear();
            }

            foreach (var entity in GetEntities())
            {
                if (IntersectsBounds(entity.Bounds, minX, maxX, minY, maxY))
                {
                    selectedEntityIds.Add(entity.Id);
                }
            }
        }

        private ISketchEntity FindEntityAtMousePosition()
        {
            if (workspaceCamera == null)
            {
                return null;
            }

            var world = workspaceCamera.ScreenToWorldPoint(Input.mousePosition);
            return FindNearestEntity(new Vector2(world.x, world.y));
        }

        private ISketchEntity FindNearestEntity(Vector2 position)
        {
            ISketchEntity bestEntity = null;
            var bestDistance = float.PositiveInfinity;

            foreach (var entity in GetEntities())
            {
                var distance = GetDistanceToBounds(position, entity.Bounds);
                if (distance <= clickToleranceMillimeters && distance < bestDistance)
                {
                    bestEntity = entity;
                    bestDistance = distance;
                }
            }

            return bestEntity;
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

        private static float GetDistanceToBounds(Vector2 position, BoundingBox bounds)
        {
            var dx = Mathf.Max(
                Mathf.Max((float)bounds.MinX - position.x, 0f),
                position.x - (float)bounds.MaxX);
            var dy = Mathf.Max(
                Mathf.Max((float)bounds.MinY - position.y, 0f),
                position.y - (float)bounds.MaxY);

            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        private static bool IntersectsBounds(BoundingBox bounds, float minX, float maxX, float minY, float maxY)
        {
            return bounds.MaxX >= minX
                && bounds.MinX <= maxX
                && bounds.MaxY >= minY
                && bounds.MinY <= maxY;
        }

        private static bool IsMultiSelectModifierPressed()
        {
            return Input.GetKey(KeyCode.LeftControl)
                || Input.GetKey(KeyCode.RightControl)
                || Input.GetKey(KeyCode.LeftShift)
                || Input.GetKey(KeyCode.RightShift);
        }
    }
}
