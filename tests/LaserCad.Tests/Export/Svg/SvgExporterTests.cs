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

        Assert.That(svg, Does.Contain("<line x1=\"1\" y1=\"2\" x2=\"3\" y2=\"4\" />"));
    }

    [Test]
    public void Export_WithRectangleEntity_ShouldWriteClosedPathElement()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch().AddEntity(new RectangleEntity(new Point2D(1, 2), 3, 4)));
        var exporter = new SvgExporter();

        string svg = exporter.Export(document);

        Assert.That(svg, Does.Contain("<path d=\"M 1 2 L 4 2 L 4 6 L 1 6 Z\" />"));
    }

    [Test]
    public void Export_WithCircleEntity_ShouldWriteCircleElement()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch().AddEntity(new CircleEntity(new Circle2D(new Point2D(2, 3), 4))));
        var exporter = new SvgExporter();

        string svg = exporter.Export(document);

        Assert.That(svg, Does.Contain("<circle cx=\"2\" cy=\"3\" r=\"4\" />"));
    }
}
