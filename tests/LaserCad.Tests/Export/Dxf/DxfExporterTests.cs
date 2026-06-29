using LaserCad.Core.Documents;
using LaserCad.Export.Dxf;

namespace LaserCad.Tests.Export.Dxf;

public class DxfExporterTests
{
    [Test]
    public void Export_WithNullDocument_ShouldThrow()
    {
        var exporter = new DxfExporter();

        Assert.Throws<ArgumentNullException>(() => exporter.Export(null!));
    }

    [Test]
    public void Export_WithEmptyDocument_ShouldWriteDxfSections()
    {
        var exporter = new DxfExporter();

        string dxf = exporter.Export(new CadDocument());

        const string expectedDxf = "0\r\nSECTION\r\n2\r\nHEADER\r\n0\r\nENDSEC\r\n0\r\nSECTION\r\n2\r\nTABLES\r\n0\r\nENDSEC\r\n0\r\nSECTION\r\n2\r\nENTITIES\r\n0\r\nENDSEC\r\n0\r\nEOF\r\n";

        Assert.That(dxf, Is.EqualTo(expectedDxf));
    }
}
