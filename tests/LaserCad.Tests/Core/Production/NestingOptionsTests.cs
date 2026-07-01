using LaserCad.Core.Production;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Production;

public sealed class NestingOptionsTests
{
    [Test]
    public void Constructor_ShouldStoreSpacing()
    {
        var options = new NestingOptions(spacing: Length.FromMillimeters(4.0));

        Assert.That(options.Spacing.Millimeters, Is.EqualTo(4.0));
    }

    [Test]
    public void Constructor_ShouldStoreAllowRotation()
    {
        var options = new NestingOptions(allowRotation: true);

        Assert.That(options.AllowRotation, Is.True);
    }
}
