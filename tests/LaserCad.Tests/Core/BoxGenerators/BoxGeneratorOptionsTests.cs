using LaserCad.Core.BoxGenerators;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.BoxGenerators;

public sealed class BoxGeneratorOptionsTests
{
    [Test]
    public void Constructor_ShouldCreateDefaultOptions()
    {
        var options = new BoxGeneratorOptions();

        Assert.That(options.Width, Is.EqualTo(Length.FromMillimeters(100.0)));
        Assert.That(options.Depth, Is.EqualTo(Length.FromMillimeters(80.0)));
        Assert.That(options.Height, Is.EqualTo(Length.FromMillimeters(50.0)));
        Assert.That(options.MaterialThickness, Is.EqualTo(Length.FromMillimeters(3.0)));
        Assert.That(options.Kerf, Is.EqualTo(Length.FromMillimeters(0.0)));
        Assert.That(options.FingerWidth, Is.EqualTo(Length.FromMillimeters(10.0)));
        Assert.That(options.Clearance, Is.EqualTo(Length.FromMillimeters(0.0)));
        Assert.That(options.BoxType, Is.EqualTo(BoxGeneratorType.Open));
    }

    [Test]
    public void Constructor_WithValues_ShouldStoreOptions()
    {
        var options = new BoxGeneratorOptions(
            width: Length.FromMillimeters(220.0),
            depth: Length.FromMillimeters(120.0),
            height: Length.FromMillimeters(90.0),
            materialThickness: Length.FromMillimeters(4.0),
            kerf: Length.FromMillimeters(0.16),
            fingerWidth: Length.FromMillimeters(12.0),
            clearance: Length.FromMillimeters(0.1),
            boxType: BoxGeneratorType.WithLid);

        Assert.That(options.Width, Is.EqualTo(Length.FromMillimeters(220.0)));
        Assert.That(options.Depth, Is.EqualTo(Length.FromMillimeters(120.0)));
        Assert.That(options.Height, Is.EqualTo(Length.FromMillimeters(90.0)));
        Assert.That(options.MaterialThickness, Is.EqualTo(Length.FromMillimeters(4.0)));
        Assert.That(options.Kerf, Is.EqualTo(Length.FromMillimeters(0.16)));
        Assert.That(options.FingerWidth, Is.EqualTo(Length.FromMillimeters(12.0)));
        Assert.That(options.Clearance, Is.EqualTo(Length.FromMillimeters(0.1)));
        Assert.That(options.BoxType, Is.EqualTo(BoxGeneratorType.WithLid));
    }

    [Test]
    public void Constructor_WithZeroWidth_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new BoxGeneratorOptions(width: Length.FromMillimeters(0.0)));
    }

    [Test]
    public void Constructor_WithNegativeKerf_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new BoxGeneratorOptions(kerf: Length.FromMillimeters(-0.1)));
    }

    [Test]
    public void Constructor_WithNegativeClearance_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new BoxGeneratorOptions(clearance: Length.FromMillimeters(-0.1)));
    }

    [Test]
    public void Constructor_WithBoxTooSmallForMaterial_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new BoxGeneratorOptions(
                width: Length.FromMillimeters(5.0),
                materialThickness: Length.FromMillimeters(3.0)));
    }
}
