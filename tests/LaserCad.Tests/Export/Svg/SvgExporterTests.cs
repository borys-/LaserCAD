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

        Assert.That(svg, Is.Not.Null);
    }
}
