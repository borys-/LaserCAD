using LaserCad.Core.Documents;
using LaserCad.Export.Dxf;
using LaserCad.Geometry;

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

    [Test]
    public void Export_WithLineEntity_ShouldWriteLineEntity()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch().AddEntity(new LineEntity(
                new LineSegment2D(new Point2D(1, 2), new Point2D(3, 4)),
                layerName: "Cut")));
        var exporter = new DxfExporter();

        string dxf = exporter.Export(document);

        Assert.That(dxf, Does.Contain("0\r\nLINE\r\n"));
        Assert.That(dxf, Does.Contain("8\r\nCut\r\n"));
        Assert.That(dxf, Does.Contain("10\r\n1\r\n20\r\n2\r\n11\r\n3\r\n21\r\n4\r\n"));
    }

    [Test]
    public void Export_WithCircleEntity_ShouldWriteCircleEntity()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch().AddEntity(new CircleEntity(
                new Circle2D(new Point2D(2, 3), 4),
                layerName: "Score")));
        var exporter = new DxfExporter();

        string dxf = exporter.Export(document);

        Assert.That(dxf, Does.Contain("0\r\nCIRCLE\r\n"));
        Assert.That(dxf, Does.Contain("8\r\nScore\r\n"));
        Assert.That(dxf, Does.Contain("10\r\n2\r\n20\r\n3\r\n40\r\n4\r\n"));
    }
}
