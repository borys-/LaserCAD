using LaserCad.Geometry;

namespace LaserCad.Tests.Geometry;

public sealed class Polyline2DTests
{
    [Test]
    public void Constructor_ShouldExposeCopiedPoints()
    {
        Point2D[] points =
        [
            new Point2D(0.0, 0.0),
            new Point2D(3.0, 0.0),
        ];

        Polyline2D polyline = new Polyline2D(points);
        points[0] = new Point2D(100.0, 100.0);

        Assert.Multiple(() =>
        {
            Assert.That(polyline.Points, Has.Count.EqualTo(2));
            Assert.That(polyline.Points[0], Is.EqualTo(new Point2D(0.0, 0.0)));
            Assert.That(polyline.IsClosed, Is.False);
        });
    }

    [Test]
    public void Constructor_WithLessThanTwoPoints_ShouldThrow()
    {
        Point2D[] points = [new Point2D(0.0, 0.0)];

        Assert.Throws<ArgumentException>(() => new Polyline2D(points));
    }

    [Test]
    public void Length_ShouldReturnSumOfSegments()
    {
        Polyline2D polyline = new Polyline2D(
        [
            new Point2D(0.0, 0.0),
            new Point2D(3.0, 0.0),
            new Point2D(3.0, 4.0),
        ]);

        Assert.That(polyline.Length, Is.EqualTo(7.0).Within(GeometryTolerance.Default));
    }

    [Test]
    public void Length_ForClosedPolyline_ShouldIncludeClosingSegment()
    {
        Polyline2D polyline = new Polyline2D(
        [
            new Point2D(0.0, 0.0),
            new Point2D(3.0, 0.0),
            new Point2D(3.0, 4.0),
        ], isClosed: true);

        Assert.Multiple(() =>
        {
            Assert.That(polyline.IsClosed, Is.True);
            Assert.That(polyline.Length, Is.EqualTo(12.0).Within(GeometryTolerance.Default));
        });
    }

    [Test]
    public void Bounds_ShouldContainAllPoints()
    {
        Polyline2D polyline = new Polyline2D(
        [
            new Point2D(2.0, 3.0),
            new Point2D(-1.0, 5.0),
            new Point2D(4.0, -2.0),
        ]);

        Assert.That(polyline.Bounds, Is.EqualTo(new BoundingBox(-1.0, -2.0, 4.0, 5.0)));
    }

    [Test]
    public void Transform_ShouldTransformAllPointsAndKeepClosedFlag()
    {
        Polyline2D polyline = new Polyline2D(
        [
            new Point2D(1.0, 2.0),
            new Point2D(3.0, 4.0),
        ], isClosed: true);
        Matrix3x3 transform = Matrix3x3.CreateTranslation(10.0, -2.0) * Matrix3x3.CreateScaling(2.0, 3.0);

        Polyline2D result = polyline.Transform(transform);

        Assert.Multiple(() =>
        {
            Assert.That(result.Points[0], Is.EqualTo(new Point2D(12.0, 4.0)));
            Assert.That(result.Points[1], Is.EqualTo(new Point2D(16.0, 10.0)));
            Assert.That(result.IsClosed, Is.True);
        });
    }
}
