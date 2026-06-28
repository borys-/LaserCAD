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
}
