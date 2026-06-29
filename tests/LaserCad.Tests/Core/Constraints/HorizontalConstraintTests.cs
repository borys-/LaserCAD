using LaserCad.Core.Constraints;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Constraints;

public sealed class HorizontalConstraintTests
{
    [Test]
    public void Solve_WithHorizontalConstraint_ShouldMakeLineHorizontal()
    {
        var line = new LineEntity(new LineSegment2D(new Point2D(1.0, 2.0), new Point2D(6.0, 9.0)));
        var sketch = new Sketch(entities: new[] { line });
        var solver = new SketchConstraintSolver();

        var solved = solver.Solve(sketch, new[] { new HorizontalConstraint(line.Id) });

        var solvedLine = (LineEntity)solved.Entities[0];
        Assert.That(solvedLine.Segment.Start, Is.EqualTo(line.Segment.Start));
        Assert.That(solvedLine.Segment.End, Is.EqualTo(new Point2D(6.0, 2.0)));
        Assert.That(solvedLine.Id, Is.EqualTo(line.Id));
        Assert.That(((LineEntity)sketch.Entities[0]).Segment.End, Is.EqualTo(new Point2D(6.0, 9.0)));
    }
}
