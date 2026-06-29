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

        private readonly HashSet<Guid> selectedEntityIds = new HashSet<Guid>();

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
                SelectAtMousePosition();
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

        private void SelectAtMousePosition()
        {
            var entity = FindEntityAtMousePosition();
            if (entity != null)
            {
                SelectOnly(entity);
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
    }
}
