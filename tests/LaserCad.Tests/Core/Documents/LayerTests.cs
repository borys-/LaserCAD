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
}
