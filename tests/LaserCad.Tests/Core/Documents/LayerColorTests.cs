using LaserCad.Core.Documents;

namespace LaserCad.Tests.Core.Documents;

public sealed class LayerColorTests
{
    [Test]
    public void FromHex_ShouldStoreUppercaseHex()
    {
        var color = LayerColor.FromHex("#ff00aa");

        Assert.That(color.Hex, Is.EqualTo("#FF00AA"));
    }

    [Test]
    public void FromHex_WithInvalidHex_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = LayerColor.FromHex("red"));
    }

    [Test]
    public void ToString_ShouldReturnHex()
    {
        var color = LayerColor.FromHex("#00AAFF");

        Assert.That(color.ToString(), Is.EqualTo("#00AAFF"));
    }
}
