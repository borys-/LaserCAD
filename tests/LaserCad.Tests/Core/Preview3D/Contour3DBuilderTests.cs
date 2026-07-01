using LaserCad.Core.Documents;
using LaserCad.Core.Preview3D;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Preview3D;

public sealed class Contour3DBuilderTests
{
    [Test]
    public void FromRectangle_ShouldCreateExtrudedPart()
    {
        var builder = new Contour3DBuilder();
        var rectangle = new RectangleEntity(new Point2D(0.0, 0.0), 20.0, 10.0);

        var part = builder.FromRectangle(rectangle, 3.0);

        Assert.That(part.ThicknessMillimeters, Is.EqualTo(3.0));
        Assert.That(part.Mesh.Vertices, Has.Count.EqualTo(8));
        Assert.That(part.Mesh.TriangleCount, Is.EqualTo(12));
    }

    [Test]
    public void FromRectangle_ShouldUseMaterialThicknessAsTopZ()
    {
        var builder = new Contour3DBuilder();
        var rectangle = new RectangleEntity(new Point2D(0.0, 0.0), 20.0, 10.0);

        var part = builder.FromRectangle(rectangle, 4.5);

        Assert.That(part.Mesh.Vertices.Select(vertex => vertex.Z), Does.Contain(0.0));
        Assert.That(part.Mesh.Vertices.Select(vertex => vertex.Z), Does.Contain(4.5));
    }

    [Test]
    public void FromPolyline_WithClosedPolygon_ShouldCreateMesh()
    {
        var builder = new Contour3DBuilder();
        var polyline = new PolylineEntity(
            new Polyline2D(
                new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(30.0, 0.0),
                    new Point2D(20.0, 15.0),
                    new Point2D(0.0, 10.0),
                },
                true));

        var part = builder.FromPolyline(polyline, 3.0);

        Assert.That(part.Mesh.Vertices, Has.Count.EqualTo(8));
        Assert.That(part.Mesh.TriangleCount, Is.EqualTo(12));
    }

    [Test]
    public void FromPolyline_WithOpenPolyline_ShouldThrow()
    {
        var builder = new Contour3DBuilder();
        var polyline = new PolylineEntity(
            new Polyline2D(
                new[]
                {
                    new Point2D(0.0, 0.0),
                    new Point2D(30.0, 0.0),
                    new Point2D(20.0, 15.0),
                },
                false));

        Assert.Throws<ArgumentException>(() => builder.FromPolyline(polyline, 3.0));
    }
}
