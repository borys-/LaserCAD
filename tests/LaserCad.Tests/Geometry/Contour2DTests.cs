using LaserCad.Geometry;

namespace LaserCad.Tests.Geometry;

public sealed class Contour2DTests
{
    [Test]
    public void Constructor_WhenFirstAndLastPointAreEqual_ShouldMarkContourAsClosed()
    {
        Contour2D contour = new Contour2D(
        [
            new Point2D(0.0, 0.0),
            new Point2D(10.0, 0.0),
            new Point2D(10.0, 5.0),
            new Point2D(0.0, 0.0),
        ]);

        Assert.That(contour.IsClosed, Is.True);
    }

    [Test]
    public void Constructor_WhenFirstAndLastPointAreDifferent_ShouldMarkContourAsOpen()
    {
        Contour2D contour = new Contour2D(
        [
            new Point2D(0.0, 0.0),
            new Point2D(10.0, 0.0),
            new Point2D(10.0, 5.0),
        ]);

        Assert.That(contour.IsClosed, Is.False);
    }

    [Test]
    public void FromPolygon_ShouldCreateClosedContour()
    {
        Polygon2D polygon = new Polygon2D(
        [
            new Point2D(0.0, 0.0),
            new Point2D(10.0, 0.0),
            new Point2D(10.0, 5.0),
            new Point2D(0.0, 5.0),
        ]);

        Contour2D contour = Contour2D.FromPolygon(polygon);

        Assert.That(contour.IsClosed, Is.True);
    }

    [Test]
    public void HasSelfIntersections_ForSimplePolygonContour_ShouldReturnFalse()
    {
        Contour2D contour = new Contour2D(
        [
            new Point2D(0.0, 0.0),
            new Point2D(10.0, 0.0),
            new Point2D(10.0, 5.0),
            new Point2D(0.0, 5.0),
            new Point2D(0.0, 0.0),
        ]);

        Assert.That(contour.HasSelfIntersections(), Is.False);
    }

    [Test]
    public void HasSelfIntersections_ForBowTieContour_ShouldReturnTrue()
    {
        Contour2D contour = new Contour2D(
        [
            new Point2D(0.0, 0.0),
            new Point2D(10.0, 10.0),
            new Point2D(0.0, 10.0),
            new Point2D(10.0, 0.0),
            new Point2D(0.0, 0.0),
        ]);

        Assert.That(contour.HasSelfIntersections(), Is.True);
    }
}
