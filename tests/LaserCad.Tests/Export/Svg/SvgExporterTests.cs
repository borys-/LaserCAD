using LaserCad.Core.Documents;
using LaserCad.Export.Svg;
using LaserCad.Geometry;

namespace LaserCad.Tests.Export.Svg;

public class SvgExporterTests
{
    [Test]
    public void Export_WithNullDocument_ShouldThrow()
    {
        var exporter = new SvgExporter();

        Assert.Throws<ArgumentNullException>(() => exporter.Export(null!));
    }

    [Test]
    public void Export_WithEmptyDocument_ShouldReturnString()
    {
        var exporter = new SvgExporter();

        string svg = exporter.Export(new CadDocument());

        const string expectedSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"0mm\" height=\"0mm\" viewBox=\"0 0 0 0\" fill=\"none\" stroke=\"#000000\" stroke-width=\"0.1\" />";

        Assert.That(svg, Is.EqualTo(expectedSvg));
    }

    [Test]
    public void Export_WithSketchEntities_ShouldUseEntityBoundsAsViewBox()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch().AddEntity(new LineEntity(
                new LineSegment2D(new Point2D(10, 20), new Point2D(40, 60)))));
        var exporter = new SvgExporter();

        string svg = exporter.Export(document);

        Assert.That(svg, Does.Contain("width=\"30mm\""));
        Assert.That(svg, Does.Contain("height=\"40mm\""));
        Assert.That(svg, Does.Contain("viewBox=\"10 20 30 40\""));
    }

    [Test]
    public void Export_WithLineEntity_ShouldWriteLineElement()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch().AddEntity(new LineEntity(
                new LineSegment2D(new Point2D(1, 2), new Point2D(3, 4)))));
        var exporter = new SvgExporter();

        string svg = exporter.Export(document);

        Assert.That(svg, Does.Contain("<line x1=\"1\" y1=\"2\" x2=\"3\" y2=\"4\" stroke=\"#000000\" />"));
    }

    [Test]
    public void Export_WithRectangleEntity_ShouldWriteClosedPathElement()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch().AddEntity(new RectangleEntity(new Point2D(1, 2), 3, 4)));
        var exporter = new SvgExporter();

        string svg = exporter.Export(document);

        Assert.That(svg, Does.Contain("<path d=\"M 1 2 L 4 2 L 4 6 L 1 6 Z\" stroke=\"#000000\" />"));
    }

    [Test]
    public void Export_WithCircleEntity_ShouldWriteCircleElement()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch().AddEntity(new CircleEntity(new Circle2D(new Point2D(2, 3), 4))));
        var exporter = new SvgExporter();

        string svg = exporter.Export(document);

        Assert.That(svg, Does.Contain("<circle cx=\"2\" cy=\"3\" r=\"4\" stroke=\"#000000\" />"));
    }

    [Test]
    public void Export_WithArcEntity_ShouldWritePathElement()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch().AddEntity(new ArcEntity(new Arc2D(
                new Point2D(0, 0),
                10,
                0,
                Math.PI / 2.0))));
        var exporter = new SvgExporter();

        string svg = exporter.Export(document);

        Assert.That(svg, Does.Contain("<path d=\"M 10 0 A 10 10 0 0 1 0 10\" stroke=\"#000000\" />"));
    }

    [Test]
    public void Export_WithPolylineEntity_ShouldWritePathElement()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch().AddEntity(new PolylineEntity(new Polyline2D(new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4),
                new Point2D(5, 6),
            }))));
        var exporter = new SvgExporter();

        string svg = exporter.Export(document);

        Assert.That(svg, Does.Contain("<path d=\"M 1 2 L 3 4 L 5 6\" stroke=\"#000000\" />"));
    }

    [Test]
    public void Export_WithLayerColor_ShouldWriteElementStroke()
    {
        var document = new CadDocument(layers: new[] { new Layer("Cut", LayerColor.FromHex("#AA1100")) })
            .AddSketch(new Sketch().AddEntity(new LineEntity(
                new LineSegment2D(new Point2D(1, 2), new Point2D(3, 4)),
                layerName: "Cut")));
        var exporter = new SvgExporter();

        string svg = exporter.Export(document);

        Assert.That(svg, Does.Contain("<line x1=\"1\" y1=\"2\" x2=\"3\" y2=\"4\" stroke=\"#AA1100\" />"));
    }

    [Test]
    public void Export_WithIgnoreLayer_ShouldSkipEntity()
    {
        var document = new CadDocument(layers: new[] { new Layer("Ignore", LayerColor.FromHex("#808080"), LayerRole.Ignore) })
            .AddSketch(new Sketch().AddEntity(new LineEntity(
                new LineSegment2D(new Point2D(10, 20), new Point2D(30, 40)),
                layerName: "Ignore")));
        var exporter = new SvgExporter();

        string svg = exporter.Export(document);

        Assert.That(svg, Does.Not.Contain("<line"));
        Assert.That(svg, Does.Contain("viewBox=\"0 0 0 0\""));
    }

    [Test]
    public void Export_WithAllSupportedEntityTypes_ShouldWriteEachSvgElement()
    {
        var sketch = new Sketch()
            .AddEntity(new LineEntity(new LineSegment2D(new Point2D(0, 0), new Point2D(10, 0))))
            .AddEntity(new RectangleEntity(new Point2D(20, 0), 10, 5))
            .AddEntity(new CircleEntity(new Circle2D(new Point2D(45, 5), 5)))
            .AddEntity(new ArcEntity(new Arc2D(new Point2D(60, 0), 10, 0, Math.PI / 2.0)))
            .AddEntity(new PolylineEntity(new Polyline2D(new[]
            {
                new Point2D(80, 0),
                new Point2D(90, 5),
                new Point2D(100, 0),
            })));
        var document = new CadDocument(layers: Array.Empty<Layer>()).AddSketch(sketch);
        var exporter = new SvgExporter();

        string svg = exporter.Export(document);

        Assert.That(svg, Does.Contain("<line x1=\"0\" y1=\"0\" x2=\"10\" y2=\"0\" stroke=\"#000000\" />"));
        Assert.That(svg, Does.Contain("<path d=\"M 20 0 L 30 0 L 30 5 L 20 5 Z\" stroke=\"#000000\" />"));
        Assert.That(svg, Does.Contain("<circle cx=\"45\" cy=\"5\" r=\"5\" stroke=\"#000000\" />"));
        Assert.That(svg, Does.Contain("<path d=\"M 70 0 A 10 10 0 0 1 60 10\" stroke=\"#000000\" />"));
        Assert.That(svg, Does.Contain("<path d=\"M 80 0 L 90 5 L 100 0\" stroke=\"#000000\" />"));
    }
}
