using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Constraints;

/// <summary>
/// Constraint wymuszajacy pionowy przebieg odcinka.
/// MVP obsluguje encje typu <see cref="LineEntity" />.
/// </summary>
public sealed class VerticalConstraint : ISketchConstraint
{
    /// <summary>
    /// Tworzy constraint pionowy dla wskazanej encji linii.
    /// </summary>
    public VerticalConstraint(Guid entityId, Guid? id = null)
    {
        if (entityId == Guid.Empty)
        {
            throw new ArgumentException("Entity id cannot be empty.", nameof(entityId));
        }

        Id = id ?? Guid.NewGuid();
        EntityId = entityId;

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Constraint id cannot be empty.", nameof(id));
        }
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <summary>
    /// Identyfikator linii objetej constraintem.
    /// </summary>
    public Guid EntityId { get; }

    /// <inheritdoc />
    public SketchConstraintKind Kind => SketchConstraintKind.Vertical;

    /// <inheritdoc />
    public Sketch Apply(Sketch sketch)
    {
        if (sketch is null)
        {
            throw new ArgumentNullException(nameof(sketch));
        }

        var line = SketchConstraintHelpers.GetLine(sketch, EntityId);
        var segment = new LineSegment2D(
            line.Segment.Start,
            new Point2D(line.Segment.Start.X, line.Segment.End.Y));
        var replacement = new LineEntity(segment, line.Id, line.LayerName);

        return SketchConstraintHelpers.ReplaceEntity(sketch, replacement);
    }
}
