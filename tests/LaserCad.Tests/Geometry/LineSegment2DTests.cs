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

    [Test]
    public void PointAt_ShouldReturnInterpolatedPoint()
    {
        LineSegment2D segment = new LineSegment2D(
            new Point2D(2.0, 4.0),
            new Point2D(10.0, 8.0));

        Point2D result = segment.PointAt(0.25);

        Assert.That(result, Is.EqualTo(new Point2D(4.0, 5.0)));
    }

    [Test]
    public void PointAt_ShouldReturnEndpointsForZeroAndOne()
    {
        LineSegment2D segment = new LineSegment2D(
            new Point2D(2.0, 4.0),
            new Point2D(10.0, 8.0));

        Assert.Multiple(() =>
        {
            Assert.That(segment.PointAt(0.0), Is.EqualTo(segment.Start));
            Assert.That(segment.PointAt(1.0), Is.EqualTo(segment.End));
        });
    }

    [Test]
    public void Direction_ShouldReturnNormalizedVectorFromStartToEnd()
    {
        LineSegment2D segment = new LineSegment2D(
            new Point2D(1.0, 2.0),
            new Point2D(4.0, 6.0));

        Vector2D direction = segment.Direction;

        Assert.Multiple(() =>
        {
            Assert.That(direction.X, Is.EqualTo(0.6).Within(GeometryTolerance.Default));
            Assert.That(direction.Y, Is.EqualTo(0.8).Within(GeometryTolerance.Default));
        });
    }

    [Test]
    public void Bounds_ShouldContainStartAndEnd()
    {
        LineSegment2D segment = new LineSegment2D(
            new Point2D(4.0, -2.0),
            new Point2D(-1.0, 6.0));

        Assert.That(segment.Bounds, Is.EqualTo(new BoundingBox(-1.0, -2.0, 4.0, 6.0)));
    }

    [Test]
    public void Transform_ShouldTransformStartAndEnd()
    {
        LineSegment2D segment = new LineSegment2D(
            new Point2D(1.0, 2.0),
            new Point2D(4.0, 6.0));
        Matrix3x3 transform = Matrix3x3.CreateTranslation(10.0, -2.0) * Matrix3x3.CreateScaling(2.0, 3.0);

        LineSegment2D result = segment.Transform(transform);

        Assert.Multiple(() =>
        {
            Assert.That(result.Start, Is.EqualTo(new Point2D(12.0, 4.0)));
            Assert.That(result.End, Is.EqualTo(new Point2D(18.0, 16.0)));
        });
    }
}
