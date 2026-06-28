using LaserCad.Core.Documents;

namespace LaserCad.Tests.Core.Documents;

public sealed class LayerTests
{
    [Test]
    public void Constructor_ShouldCreateLayer()
    {
        var layer = new Layer("Cut");

        Assert.That(layer, Is.Not.Null);
    }

    [Test]
    public void Constructor_ShouldStoreName()
    {
        var layer = new Layer("Cut");

        Assert.That(layer.Name, Is.EqualTo("Cut"));
    }

    [Test]
    public void Constructor_WithEmptyName_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = new Layer(""));
    }

    [Test]
    public void Constructor_ShouldUseDefaultColor()
    {
        var layer = new Layer("Cut");

        Assert.That(layer.Color, Is.EqualTo(LayerColor.Black));
    }

    [Test]
    public void Constructor_WithColor_ShouldStoreColor()
    {
        var color = LayerColor.FromHex("#FF0000");

        var layer = new Layer("Cut", color);

        Assert.That(layer.Color, Is.SameAs(color));
    }

    [Test]
    public void Constructor_ShouldUseDefaultRole()
    {
        var layer = new Layer("Cut");

        Assert.That(layer.Role, Is.EqualTo(LayerRole.Cut));
    }

    [Test]
    public void Constructor_WithRole_ShouldStoreRole()
    {
        var layer = new Layer("Engrave", role: LayerRole.Engrave);

        Assert.That(layer.Role, Is.EqualTo(LayerRole.Engrave));
    }
}
