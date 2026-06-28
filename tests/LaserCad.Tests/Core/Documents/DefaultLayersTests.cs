using LaserCad.Core.Documents;

namespace LaserCad.Tests.Core.Documents;

public sealed class DefaultLayersTests
{
    [Test]
    public void All_ShouldContainProductionLayers()
    {
        Assert.That(DefaultLayers.All, Has.Count.EqualTo(4));
        Assert.That(DefaultLayers.All, Is.EqualTo(new[] { DefaultLayers.Cut, DefaultLayers.Engrave, DefaultLayers.Score, DefaultLayers.Ignore }));
    }

    [Test]
    public void All_ShouldDefineNamesColorsAndRoles()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DefaultLayers.Cut.Name, Is.EqualTo("Cut"));
            Assert.That(DefaultLayers.Cut.Color.Hex, Is.EqualTo("#FF0000"));
            Assert.That(DefaultLayers.Cut.Role, Is.EqualTo(LayerRole.Cut));

            Assert.That(DefaultLayers.Engrave.Name, Is.EqualTo("Engrave"));
            Assert.That(DefaultLayers.Engrave.Color.Hex, Is.EqualTo("#0000FF"));
            Assert.That(DefaultLayers.Engrave.Role, Is.EqualTo(LayerRole.Engrave));

            Assert.That(DefaultLayers.Score.Name, Is.EqualTo("Score"));
            Assert.That(DefaultLayers.Score.Color.Hex, Is.EqualTo("#00AA00"));
            Assert.That(DefaultLayers.Score.Role, Is.EqualTo(LayerRole.Score));

            Assert.That(DefaultLayers.Ignore.Name, Is.EqualTo("Ignore"));
            Assert.That(DefaultLayers.Ignore.Color.Hex, Is.EqualTo("#808080"));
            Assert.That(DefaultLayers.Ignore.Role, Is.EqualTo(LayerRole.Ignore));
        });
    }

    [Test]
    public void CadDocument_ShouldUseDefaultLayers()
    {
        var document = new CadDocument();

        Assert.That(document.Layers, Is.EqualTo(DefaultLayers.All));
    }
}
