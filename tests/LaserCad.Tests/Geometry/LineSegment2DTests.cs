using LaserCad.Geometry;

namespace LaserCad.Tests.Geometry;

public sealed class LineSegment2DTests
{
    [Test]
    public void Length_ShouldReturnDistanceBetweenStartAndEnd()
    {
        LineSegment2D segment = new LineSegment2D(
            new Point2D(1.0, 2.0),
            new Point2D(4.0, 6.0));

        Assert.That(segment.Length, Is.EqualTo(5.0).Within(GeometryTolerance.Default));
    }
}
