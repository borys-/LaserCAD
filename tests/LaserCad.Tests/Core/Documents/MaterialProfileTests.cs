using LaserCad.Core.Documents;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Documents;

public sealed class MaterialProfileTests
{
    [Test]
    public void Constructor_ShouldCreateMaterialProfile()
    {
        var profile = new MaterialProfile("Plywood 3 mm");

        Assert.That(profile, Is.Not.Null);
    }

    [Test]
    public void Constructor_ShouldStoreName()
    {
        var profile = new MaterialProfile("Plywood 3 mm");

        Assert.That(profile.Name, Is.EqualTo("Plywood 3 mm"));
    }

    [Test]
    public void Constructor_WithEmptyName_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = new MaterialProfile(""));
    }

    [Test]
    public void Constructor_ShouldUseDefaultThickness()
    {
        var profile = new MaterialProfile("Custom");

        Assert.That(profile.Thickness, Is.EqualTo(Length.FromMillimeters(0.0)));
    }

    [Test]
    public void Constructor_WithThickness_ShouldStoreThickness()
    {
        var thickness = Length.FromMillimeters(3.0);

        var profile = new MaterialProfile("Plywood 3 mm", thickness);

        Assert.That(profile.Thickness, Is.EqualTo(thickness));
    }

    [Test]
    public void Constructor_WithNegativeThickness_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new MaterialProfile("Invalid", Length.FromMillimeters(-1.0)));
    }

    [Test]
    public void Constructor_ShouldUseDefaultKerf()
    {
        var profile = new MaterialProfile("Custom");

        Assert.That(profile.DefaultKerf, Is.EqualTo(Length.FromMillimeters(0.0)));
    }

    [Test]
    public void Constructor_WithDefaultKerf_ShouldStoreDefaultKerf()
    {
        var kerf = Length.FromMillimeters(0.15);

        var profile = new MaterialProfile("Plywood 3 mm", defaultKerf: kerf);

        Assert.That(profile.DefaultKerf, Is.EqualTo(kerf));
    }

    [Test]
    public void Constructor_WithNegativeDefaultKerf_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new MaterialProfile("Invalid", defaultKerf: Length.FromMillimeters(-0.1)));
    }

    [Test]
    public void Constructor_ShouldUseDefaultClearance()
    {
        var profile = new MaterialProfile("Custom");

        Assert.That(profile.DefaultClearance, Is.EqualTo(Length.FromMillimeters(0.0)));
    }

    [Test]
    public void Constructor_WithDefaultClearance_ShouldStoreDefaultClearance()
    {
        var clearance = Length.FromMillimeters(0.1);

        var profile = new MaterialProfile("Plywood 3 mm", defaultClearance: clearance);

        Assert.That(profile.DefaultClearance, Is.EqualTo(clearance));
    }

    [Test]
    public void Constructor_WithNegativeDefaultClearance_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new MaterialProfile("Invalid", defaultClearance: Length.FromMillimeters(-0.1)));
    }

    [Test]
    public void Constructor_ShouldUseDefaultMinimumFingerWidth()
    {
        var profile = new MaterialProfile("Custom");

        Assert.That(profile.MinimumFingerWidth, Is.EqualTo(Length.FromMillimeters(0.0)));
    }

    [Test]
    public void Constructor_WithMinimumFingerWidth_ShouldStoreMinimumFingerWidth()
    {
        var minimumFingerWidth = Length.FromMillimeters(6.0);

        var profile = new MaterialProfile("Plywood 3 mm", minimumFingerWidth: minimumFingerWidth);

        Assert.That(profile.MinimumFingerWidth, Is.EqualTo(minimumFingerWidth));
    }

    [Test]
    public void Constructor_WithNegativeMinimumFingerWidth_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => _ = new MaterialProfile("Invalid", minimumFingerWidth: Length.FromMillimeters(-1.0)));
    }
}
