using LaserCad.Export.Svg;

namespace LaserCad.Tests.Export.Svg;

public class SvgExportOptionsTests
{
    [Test]
    public void Default_ShouldUseMillimeters()
    {
        var options = new SvgExportOptions();

        Assert.That(options.Unit, Is.EqualTo(SvgExportUnit.Millimeters));
    }

    [Test]
    public void StrokeWidthMillimeters_WithNegativeValue_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new SvgExportOptions
        {
            StrokeWidthMillimeters = -0.1,
        });
    }

    [Test]
    public void ExportedLayerNames_ShouldTrimEmptyNamesAndDuplicates()
    {
        var options = new SvgExportOptions
        {
            ExportedLayerNames = new[] { " Cut ", "", "Cut", "Engrave" },
        };

        Assert.That(options.ExportedLayerNames, Is.EqualTo(new[] { "Cut", "Engrave" }));
    }
}
