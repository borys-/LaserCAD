using LaserCad.Core.FingerJoints;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.FingerJoints;

public sealed class FingerJointOptionsTests
{
    [Test]
    public void Constructor_ShouldCreateDefaultOptions()
    {
        var options = new FingerJointOptions();

        Assert.That(options.FingerWidth, Is.EqualTo(Length.FromMillimeters(1.0)));
        Assert.That(options.MinimumFingerWidth, Is.EqualTo(Length.FromMillimeters(1.0)));
        Assert.That(options.MaximumFingerWidth, Is.EqualTo(Length.FromMillimeters(1.0)));
        Assert.That(options.StartWithFinger, Is.True);
        Assert.That(options.EndWithFinger, Is.True);
        Assert.That(options.FitMode, Is.EqualTo(FingerJointFitMode.Neutral));
        Assert.That(options.Kerf, Is.EqualTo(Length.FromMillimeters(0.0)));
        Assert.That(options.Clearance, Is.EqualTo(Length.FromMillimeters(0.0)));
    }

    [Test]
    public void Constructor_WithValues_ShouldStoreOptions()
    {
        var options = new FingerJointOptions(
            fingerWidth: Length.FromMillimeters(8.0),
            minimumFingerWidth: Length.FromMillimeters(5.0),
            maximumFingerWidth: Length.FromMillimeters(12.0),
            startWithFinger: false,
            endWithFinger: false,
            fitMode: FingerJointFitMode.Loose,
            kerf: Length.FromMillimeters(0.15),
            clearance: Length.FromMillimeters(0.1));

        Assert.That(options.FingerWidth, Is.EqualTo(Length.FromMillimeters(8.0)));
        Assert.That(options.MinimumFingerWidth, Is.EqualTo(Length.FromMillimeters(5.0)));
        Assert.That(options.MaximumFingerWidth, Is.EqualTo(Length.FromMillimeters(12.0)));
        Assert.That(options.StartWithFinger, Is.False);
        Assert.That(options.EndWithFinger, Is.False);
        Assert.That(options.FitMode, Is.EqualTo(FingerJointFitMode.Loose));
        Assert.That(options.Kerf, Is.EqualTo(Length.FromMillimeters(0.15)));
        Assert.That(options.Clearance, Is.EqualTo(Length.FromMillimeters(0.1)));
    }

    [Test]
    public void Constructor_WithZeroFingerWidth_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new FingerJointOptions(fingerWidth: Length.FromMillimeters(0.0)));
    }

    [Test]
    public void Constructor_WithNegativeMinimumFingerWidth_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new FingerJointOptions(minimumFingerWidth: Length.FromMillimeters(-1.0)));
    }

    [Test]
    public void Constructor_WithMinimumGreaterThanMaximum_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(
            () => _ = new FingerJointOptions(
                fingerWidth: Length.FromMillimeters(6.0),
                minimumFingerWidth: Length.FromMillimeters(8.0),
                maximumFingerWidth: Length.FromMillimeters(4.0)));
    }

    [Test]
    public void Constructor_WithFingerWidthOutsideRange_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new FingerJointOptions(
                fingerWidth: Length.FromMillimeters(12.0),
                minimumFingerWidth: Length.FromMillimeters(4.0),
                maximumFingerWidth: Length.FromMillimeters(10.0)));
    }

    [Test]
    public void Constructor_WithNegativeKerf_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new FingerJointOptions(kerf: Length.FromMillimeters(-0.1)));
    }

    [Test]
    public void Constructor_WithNegativeClearance_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new FingerJointOptions(clearance: Length.FromMillimeters(-0.1)));
    }
}
