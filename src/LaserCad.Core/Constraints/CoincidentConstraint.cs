using LaserCad.Core.Documents;

namespace LaserCad.Core.Constraints;

/// <summary>
/// Constraint laczacy dwa punkty szkicu w tym samym miejscu.
/// MVP przesuwa drugi punkt do polozenia pierwszego.
/// </summary>
public sealed class CoincidentConstraint : ISketchConstraint
{
    public CoincidentConstraint(
        SketchPointReference referencePoint,
        SketchPointReference constrainedPoint,
        Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        ReferencePoint = referencePoint;
        ConstrainedPoint = constrainedPoint;

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Constraint id cannot be empty.", nameof(id));
        }
    }

    public Guid Id { get; }

    public SketchPointReference ReferencePoint { get; }

    public SketchPointReference ConstrainedPoint { get; }

    public SketchConstraintKind Kind => SketchConstraintKind.Coincident;

    public Sketch Apply(Sketch sketch)
    {
        if (sketch is null)
        {
            throw new ArgumentNullException(nameof(sketch));
        }

        var point = SketchConstraintHelpers.GetPoint(sketch, ReferencePoint);

        return SketchConstraintHelpers.MovePoint(sketch, ConstrainedPoint, point);
    }
}
