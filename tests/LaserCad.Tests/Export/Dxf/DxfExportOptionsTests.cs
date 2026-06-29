using LaserCad.Export.Dxf;

namespace LaserCad.Tests.Export.Dxf;

public class DxfExportOptionsTests
{
    [Test]
    public void DefaultUnit_ShouldBeMillimeters()
    {
        var options = new DxfExportOptions();

        Assert.That(options.Unit, Is.EqualTo(DxfExportUnit.Millimeters));
    }

    [Test]
    public void ExportedLayerNames_ShouldNormalizeLayerNames()
    {
        var options = new DxfExportOptions
        {
            ExportedLayerNames = new[] { " Cut ", "", "Cut", "Engrave" },
        };

        Assert.That(options.ExportedLayerNames, Is.EqualTo(new[] { "Cut", "Engrave" }));
    }
}
