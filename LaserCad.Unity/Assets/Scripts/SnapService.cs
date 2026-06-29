using System.Collections.Generic;
using LaserCad.Core.Documents;
using LaserCad.Geometry;
using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Wyznacza najblizszy punkt snapowania w obszarze roboczym.
    /// </summary>
    public sealed class SnapService : MonoBehaviour
    {
        [SerializeField]
        private bool snapToGrid = true;

        [SerializeField]
        private float gridStepMillimeters = 1f;

        [SerializeField]
        private float snapRadiusMillimeters = 2.5f;

        /// <summary>
        /// Okresla, czy snap do siatki jest aktywny.
        /// </summary>
        public bool SnapToGridEnabled
        {
            get { return snapToGrid; }
            set { snapToGrid = value; }
        }

        /// <summary>
        /// Krok snapowania do siatki w milimetrach.
        /// </summary>
        public float GridStepMillimeters
        {
            get { return gridStepMillimeters; }
            set { gridStepMillimeters = Mathf.Max(0.001f, value); }
        }

        /// <summary>
        /// Przyciaga pozycje do najlepszego kandydata snapowania.
        /// </summary>
        public SnapResult Snap(Vector2 worldPosition, IEnumerable<ISketchEntity> entities)
        {
            var best = new SnapResult(worldPosition, false, SnapPriority.Grid);
            var bestDistance = snapRadiusMillimeters;

            if (snapToGrid)
            {
                TrySelectCandidate(
                    new SnapCandidate(SnapToGrid(worldPosition), SnapPriority.Grid),
                    worldPosition,
                    ref best,
                    ref bestDistance);
            }

            foreach (var candidate in GetEntityPointCandidates(entities))
            {
                TrySelectCandidate(candidate, worldPosition, ref best, ref bestDistance);
            }

            return best;
        }

        private Vector2 SnapToGrid(Vector2 worldPosition)
        {
            var step = Mathf.Max(0.001f, gridStepMillimeters);
            return new Vector2(
                Mathf.Round(worldPosition.x / step) * step,
                Mathf.Round(worldPosition.y / step) * step);
        }

        private static void TrySelectCandidate(
            SnapCandidate candidate,
            Vector2 sourcePosition,
            ref SnapResult best,
            ref float bestDistance)
        {
            var distance = Vector2.Distance(sourcePosition, candidate.Position);
            if (distance > bestDistance)
            {
                return;
            }

            if (best.HasSnap && candidate.Priority < best.Priority && Mathf.Approximately(distance, bestDistance))
            {
                return;
            }

            best = new SnapResult(candidate.Position, true, candidate.Priority);
            bestDistance = distance;
        }

        private static IEnumerable<SnapCandidate> GetEntityPointCandidates(IEnumerable<ISketchEntity> entities)
        {
            if (entities == null)
            {
                yield break;
            }

            foreach (var entity in entities)
            {
                if (entity is RectangleEntity rectangle)
                {
                    foreach (var corner in rectangle.Corners)
                    {
                        yield return new SnapCandidate(ToVector2(corner), SnapPriority.EntityPoint);
                    }

                    yield return new SnapCandidate(GetBoundsCenter(rectangle.Bounds), SnapPriority.EntityCenter);
                }
                else if (entity is CircleEntity circle)
                {
                    yield return new SnapCandidate(ToVector2(circle.Circle.Center), SnapPriority.EntityCenter);
                }
                else if (entity is PolylineEntity polyline)
                {
                    foreach (var point in polyline.Polyline.Points)
                    {
                        yield return new SnapCandidate(ToVector2(point), SnapPriority.EntityPoint);
                    }
                }
                else if (entity is TextEntity text)
                {
                    yield return new SnapCandidate(ToVector2(text.Position), SnapPriority.EntityPoint);
                }
                else if (entity is ArcEntity arc)
                {
                    yield return new SnapCandidate(ToVector2(arc.Arc.PointAt(0.0)), SnapPriority.EntityPoint);
                    yield return new SnapCandidate(ToVector2(arc.Arc.PointAt(1.0)), SnapPriority.EntityPoint);
                }
            }
        }

        private static Vector2 ToVector2(Point2D point)
        {
            return new Vector2((float)point.X, (float)point.Y);
        }

        private static Vector2 GetBoundsCenter(BoundingBox bounds)
        {
            return new Vector2(
                (float)((bounds.MinX + bounds.MaxX) * 0.5),
                (float)((bounds.MinY + bounds.MaxY) * 0.5));
        }
    }
}
