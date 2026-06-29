using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Wynik snapowania pozycji w obszarze roboczym.
    /// </summary>
    public readonly struct SnapResult
    {
        public SnapResult(Vector2 position, bool hasSnap, SnapPriority priority)
        {
            Position = position;
            HasSnap = hasSnap;
            Priority = priority;
        }

        public Vector2 Position { get; }

        public bool HasSnap { get; }

        public SnapPriority Priority { get; }
    }
}
