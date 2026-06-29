using LaserCad.Geometry;

namespace LaserCad.Tests.Geometry;

public sealed class Intersections2DTests
{
    [Test]
    public void IntersectSegments_CrossingSegments_ShouldReturnIntersectionPoint()
    {
        LineSegment2D horizontal = new LineSegment2D(
            new Point2D(0.0, 5.0),
            new Point2D(10.0, 5.0));
        LineSegment2D vertical = new LineSegment2D(
            new Point2D(4.0, 0.0),
            new Point2D(4.0, 10.0));

        IntersectionResult result = Intersections2D.Intersect(horizontal, vertical);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsPoint, Is.True);
            Assert.That(result.Point, Is.EqualTo(new Point2D(4.0, 5.0)));
        });
    }

    [Test]
    public void IntersectSegments_DisjointSegments_ShouldReturnNoIntersection()
    {
        LineSegment2D left = new LineSegment2D(
            new Point2D(0.0, 0.0),
            new Point2D(2.0, 0.0));
        LineSegment2D right = new LineSegment2D(
            new Point2D(3.0, 1.0),
            new Point2D(3.0, 4.0));

        IntersectionResult result = Intersections2D.Intersect(left, right);

        Assert.That(result.IsNone, Is.True);
    }
}
