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

        Assert.That(svg, Does.StartWith("<svg"));
        Assert.That(svg, Does.Contain("xmlns=\"http://www.w3.org/2000/svg\""));
        Assert.That(svg, Does.Contain("width=\"0mm\""));
        Assert.That(svg, Does.Contain("height=\"0mm\""));
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
}
