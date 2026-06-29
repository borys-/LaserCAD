namespace LaserCad.Core.Constraints;

/// <summary>
/// Referencja do punktu geometrycznego encji szkicu.
/// </summary>
public readonly record struct SketchPointReference
{
    public SketchPointReference(Guid entityId, SketchPointRole pointRole)
    {
        if (entityId == Guid.Empty)
        {
            throw new ArgumentException("Entity id cannot be empty.", nameof(entityId));
        }

        EntityId = entityId;
        PointRole = pointRole;
    }

    /// <summary>
    /// Identyfikator encji, ktora zawiera punkt.
    /// </summary>
    public Guid EntityId { get; }

    /// <summary>
    /// Rola punktu w encji.
    /// </summary>
    public SketchPointRole PointRole { get; }
}
