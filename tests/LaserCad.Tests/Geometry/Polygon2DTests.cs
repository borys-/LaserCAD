using LaserCad.Geometry;

namespace LaserCad.Tests.Geometry;

public sealed class Polygon2DTests
{
    [Test]
    public void Constructor_ShouldExposeCopiedVertices()
    {
        Point2D[] vertices =
        [
            new Point2D(0.0, 0.0),
            new Point2D(4.0, 0.0),
            new Point2D(4.0, 3.0),
        ];

        Polygon2D polygon = new Polygon2D(vertices);
        vertices[0] = new Point2D(100.0, 100.0);

        Assert.Multiple(() =>
        {
            Assert.That(polygon.Vertices, Has.Count.EqualTo(3));
            Assert.That(polygon.Vertices[0], Is.EqualTo(new Point2D(0.0, 0.0)));
        });
    }

    [Test]
    public void Constructor_WithLessThanThreeVertices_ShouldThrow()
    {
        Point2D[] vertices =
        [
            new Point2D(0.0, 0.0),
            new Point2D(1.0, 0.0),
        ];

        Assert.Throws<ArgumentException>(() => new Polygon2D(vertices));
    }

    [Test]
    public void Constructor_WithZeroArea_ShouldThrow()
    {
        Point2D[] vertices =
        [
            new Point2D(0.0, 0.0),
            new Point2D(1.0, 0.0),
            new Point2D(2.0, 0.0),
        ];

        Assert.Throws<ArgumentException>(() => new Polygon2D(vertices));
    }

    [Test]
    public void Area_ShouldReturnRectangleArea()
    {
        Polygon2D polygon = new Polygon2D(
        [
            new Point2D(0.0, 0.0),
            new Point2D(4.0, 0.0),
            new Point2D(4.0, 3.0),
            new Point2D(0.0, 3.0),
        ]);

        Assert.That(polygon.Area, Is.EqualTo(12.0).Within(GeometryTolerance.Default));
    }

    [Test]
    public void Orientation_ShouldReturnCounterclockwiseForPositiveSignedArea()
    {
        Polygon2D polygon = new Polygon2D(
        [
            new Point2D(0.0, 0.0),
            new Point2D(4.0, 0.0),
            new Point2D(4.0, 3.0),
            new Point2D(0.0, 3.0),
        ]);

        Assert.That(polygon.Orientation, Is.EqualTo(PolygonOrientation.Counterclockwise));
    }

    [Test]
    public void Orientation_ShouldReturnClockwiseForNegativeSignedArea()
    {
        Polygon2D polygon = new Polygon2D(
        [
            new Point2D(0.0, 0.0),
            new Point2D(0.0, 3.0),
            new Point2D(4.0, 3.0),
            new Point2D(4.0, 0.0),
        ]);

        Assert.That(polygon.Orientation, Is.EqualTo(PolygonOrientation.Clockwise));
    }

    [Test]
    public void Bounds_ShouldContainAllVertices()
    {
        Polygon2D polygon = new Polygon2D(
        [
            new Point2D(2.0, 3.0),
            new Point2D(-1.0, 5.0),
            new Point2D(4.0, -2.0),
        ]);

        Assert.That(polygon.Bounds, Is.EqualTo(new BoundingBox(-1.0, -2.0, 4.0, 5.0)));
    }

    [Test]
    public void Transform_ShouldTransformAllVertices()
    {
        Polygon2D polygon = new Polygon2D(
        [
            new Point2D(1.0, 2.0),
            new Point2D(3.0, 2.0),
            new Point2D(3.0, 4.0),
        ]);
        Matrix3x3 transform = Matrix3x3.CreateTranslation(10.0, -2.0) * Matrix3x3.CreateScaling(2.0, 3.0);

        Polygon2D result = polygon.Transform(transform);

        Assert.Multiple(() =>
        {
            Assert.That(result.Vertices[0], Is.EqualTo(new Point2D(12.0, 4.0)));
            Assert.That(result.Vertices[1], Is.EqualTo(new Point2D(16.0, 4.0)));
            Assert.That(result.Vertices[2], Is.EqualTo(new Point2D(16.0, 10.0)));
        });
    }
}
