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
}
