using LaserCad.Geometry;

namespace LaserCad.Tests.Geometry;

public sealed class PolygonOffset2DTests
{
    [Test]
    public void OffsetOuter_ForCounterclockwiseSquare_ShouldExpandSquare()
    {
        Polygon2D square = CreateSquare();

        Polygon2D result = PolygonOffset2D.OffsetOuter(square, 1.0);

        Assert.Multiple(() =>
        {
            Assert.That(result.Vertices[0].X, Is.EqualTo(-1.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[0].Y, Is.EqualTo(-1.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[1].X, Is.EqualTo(11.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[1].Y, Is.EqualTo(-1.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[2].X, Is.EqualTo(11.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[2].Y, Is.EqualTo(11.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[3].X, Is.EqualTo(-1.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[3].Y, Is.EqualTo(11.0).Within(GeometryTolerance.Default));
        });
    }

    [Test]
    public void OffsetInner_ForCounterclockwiseSquare_ShouldShrinkSquare()
    {
        Polygon2D square = CreateSquare();

        Polygon2D result = PolygonOffset2D.OffsetInner(square, 1.0);

        Assert.Multiple(() =>
        {
            Assert.That(result.Vertices[0].X, Is.EqualTo(1.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[0].Y, Is.EqualTo(1.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[1].X, Is.EqualTo(9.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[1].Y, Is.EqualTo(1.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[2].X, Is.EqualTo(9.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[2].Y, Is.EqualTo(9.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[3].X, Is.EqualTo(1.0).Within(GeometryTolerance.Default));
            Assert.That(result.Vertices[3].Y, Is.EqualTo(9.0).Within(GeometryTolerance.Default));
        });
    }

    [Test]
    public void OffsetOuter_ForConcavePolygon_ShouldThrow()
    {
        Polygon2D polygon = new Polygon2D(
        [
            new Point2D(0.0, 0.0),
            new Point2D(4.0, 0.0),
            new Point2D(4.0, 4.0),
            new Point2D(2.0, 2.0),
            new Point2D(0.0, 4.0),
        ]);

        Assert.Throws<ArgumentException>(() => PolygonOffset2D.OffsetOuter(polygon, 1.0));
    }

    private static Polygon2D CreateSquare()
    {
        return new Polygon2D(
        [
            new Point2D(0.0, 0.0),
            new Point2D(10.0, 0.0),
            new Point2D(10.0, 10.0),
            new Point2D(0.0, 10.0),
        ]);
    }
}
