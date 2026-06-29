using LaserCad.Core.Documents;
using LaserCad.Export.Svg;

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
}
