using LaserCad.Core.Documents;
using LaserCad.Core.FeatureTree;

namespace LaserCad.Tests.Core.FeatureTree;

public sealed class FeatureTreeItemTests
{
    [Test]
    public void Constructor_WithEmptyName_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new TestFeatureTreeItem(name: " "));
    }

    [Test]
    public void Constructor_WithEmptyId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => new TestFeatureTreeItem(id: Guid.Empty));
    }

    [Test]
    public void Constructor_ShouldStoreBasicProperties()
    {
        var id = Guid.NewGuid();

        var item = new TestFeatureTreeItem(id, "Base feature");

        Assert.That(item.Id, Is.EqualTo(id));
        Assert.That(item.Name, Is.EqualTo("Base feature"));
        Assert.That(item.Kind, Is.EqualTo(FeatureTreeItemKind.Generator));
        Assert.That(item.IsEnabled, Is.True);
    }

    private sealed class TestFeatureTreeItem : FeatureTreeItem
    {
        public TestFeatureTreeItem(Guid? id = null, string name = "Feature", bool isEnabled = true)
            : base(id, name, FeatureTreeItemKind.Generator, isEnabled)
        {
        }

        public override FeatureTreeItem WithEnabled(bool isEnabled)
        {
            return new TestFeatureTreeItem(Id, Name, isEnabled);
        }

        public override CadDocument Apply(CadDocument document)
        {
            return document;
        }
    }
}
