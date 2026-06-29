using LaserCad.Core.Constraints;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Constraints;

public sealed class CoincidentConstraintTests
{
    [Test]
    public void Solve_WithCoincidentConstraint_ShouldMoveConstrainedPointToReferencePoint()
    {
        var reference = new LineEntity(new LineSegment2D(new Point2D(1.0, 2.0), new Point2D(8.0, 9.0)));
        var constrained = new LineEntity(new LineSegment2D(new Point2D(20.0, 30.0), new Point2D(40.0, 50.0)));
        var sketch = new Sketch(entities: new[] { reference, constrained });
        var solver = new SketchConstraintSolver();
        var constraint = new CoincidentConstraint(
            new SketchPointReference(reference.Id, SketchPointRole.End),
            new SketchPointReference(constrained.Id, SketchPointRole.Start));

        var solved = solver.Solve(sketch, new[] { constraint });

        var solvedReference = (LineEntity)solved.Entities[0];
        var solvedConstrained = (LineEntity)solved.Entities[1];
        Assert.That(solvedReference.Segment, Is.EqualTo(reference.Segment));
        Assert.That(solvedConstrained.Segment.Start, Is.EqualTo(reference.Segment.End));
        Assert.That(solvedConstrained.Segment.End, Is.EqualTo(constrained.Segment.End));
        Assert.That(((LineEntity)sketch.Entities[1]).Segment.Start, Is.EqualTo(new Point2D(20.0, 30.0)));
    }
}
