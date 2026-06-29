using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Constraints;

/// <summary>
/// Constraint wymuszajacy prostopadlosc dwoch odcinkow.
/// MVP dopasowuje drugi odcinek do kierunku prostopadlego wzgledem pierwszego.
/// </summary>
public sealed class PerpendicularConstraint : ISketchConstraint
{
    public PerpendicularConstraint(Guid referenceEntityId, Guid constrainedEntityId, Guid? id = null)
    {
        if (referenceEntityId == Guid.Empty)
        {
            throw new ArgumentException("Reference entity id cannot be empty.", nameof(referenceEntityId));
        }

        if (constrainedEntityId == Guid.Empty)
        {
            throw new ArgumentException("Constrained entity id cannot be empty.", nameof(constrainedEntityId));
        }

        Id = id ?? Guid.NewGuid();
        ReferenceEntityId = referenceEntityId;
        ConstrainedEntityId = constrainedEntityId;

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Constraint id cannot be empty.", nameof(id));
        }
    }

    public Guid Id { get; }

    public Guid ReferenceEntityId { get; }

    public Guid ConstrainedEntityId { get; }

    public SketchConstraintKind Kind => SketchConstraintKind.Perpendicular;

    public Sketch Apply(Sketch sketch)
    {
        if (sketch is null)
        {
            throw new ArgumentNullException(nameof(sketch));
        }

        var reference = SketchConstraintHelpers.GetLine(sketch, ReferenceEntityId);
        var constrained = SketchConstraintHelpers.GetLine(sketch, ConstrainedEntityId);
        Vector2D direction = reference.Segment.Direction;
        var perpendicular = new Vector2D(-direction.Y, direction.X);
        var end = new Point2D(
            constrained.Segment.Start.X + (perpendicular.X * constrained.Segment.Length),
            constrained.Segment.Start.Y + (perpendicular.Y * constrained.Segment.Length));
        var replacement = new LineEntity(
            new LineSegment2D(constrained.Segment.Start, end),
            constrained.Id,
            constrained.LayerName);

        return SketchConstraintHelpers.ReplaceEntity(sketch, replacement);
    }
}
