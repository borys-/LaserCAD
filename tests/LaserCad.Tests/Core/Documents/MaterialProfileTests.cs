using LaserCad.Core.Documents;

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
}
