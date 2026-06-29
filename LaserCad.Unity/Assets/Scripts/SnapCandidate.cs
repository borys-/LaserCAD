using UnityEngine;

namespace LaserCad.Unity
{
    /// <summary>
    /// Opisuje pojedynczy punkt, do ktorego mozna przyciagnac kursor.
    /// </summary>
    public readonly struct SnapCandidate
    {
        public SnapCandidate(Vector2 position, SnapPriority priority)
        {
            Position = position;
            Priority = priority;
        }

        public Vector2 Position { get; }

        public SnapPriority Priority { get; }
    }
}
