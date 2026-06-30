using LaserCad.Core.Kerf;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Kerf;

public sealed class KerfCompensationOptionsTests
{
    [Test]
    public void Constructor_WithKerf_ShouldExposeHalfKerfOffset()
    {
        var options = new KerfCompensationOptions(Length.FromMillimeters(0.2));

        Assert.That(options.Kerf.Millimeters, Is.EqualTo(0.2));
        Assert.That(options.OffsetDistanceMillimeters, Is.EqualTo(0.1));
        Assert.That(options.Mode, Is.EqualTo(KerfCompensationMode.Positive));
    }

    [Test]
    public void Constructor_WithNegativeKerf_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new KerfCompensationOptions(Length.FromMillimeters(-0.1)));
    }
}
