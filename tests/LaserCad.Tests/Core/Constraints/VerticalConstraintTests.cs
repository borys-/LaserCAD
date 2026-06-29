using LaserCad.Core.Constraints;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Constraints;

public sealed class VerticalConstraintTests
{
    [Test]
    public void Solve_WithVerticalConstraint_ShouldMakeLineVertical()
    {
        var line = new LineEntity(new LineSegment2D(new Point2D(3.0, 4.0), new Point2D(10.0, 14.0)));
        var sketch = new Sketch(entities: new[] { line });
        var solver = new SketchConstraintSolver();

        var solved = solver.Solve(sketch, new[] { new VerticalConstraint(line.Id) });

        var solvedLine = (LineEntity)solved.Entities[0];
        Assert.That(solvedLine.Segment.Start, Is.EqualTo(line.Segment.Start));
        Assert.That(solvedLine.Segment.End, Is.EqualTo(new Point2D(3.0, 14.0)));
        Assert.That(solvedLine.Id, Is.EqualTo(line.Id));
        Assert.That(((LineEntity)sketch.Entities[0]).Segment.End, Is.EqualTo(new Point2D(10.0, 14.0)));
    }
}
