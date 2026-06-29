using System.Collections.Generic;
using LaserCad.Core.Documents;
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
    }
}
