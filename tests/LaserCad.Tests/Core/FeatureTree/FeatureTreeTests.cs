using LaserCad.Core.Documents;
using LaserCad.Core.FeatureTree;

namespace LaserCad.Tests.Core.FeatureTree;

public sealed class FeatureTreeTests
{
    [Test]
    public void Add_ShouldAppendItem()
    {
        var item = CreateItem("Feature");

        var tree = new LaserCad.Core.FeatureTree.FeatureTree().Add(item);

        Assert.That(tree.Items, Has.Count.EqualTo(1));
        Assert.That(tree.Items[0], Is.SameAs(item));
    }

    [Test]
    public void Disable_ShouldReturnTreeWithDisabledItem()
    {
        var item = CreateItem("Feature");
        var tree = new LaserCad.Core.FeatureTree.FeatureTree(new[] { item });

        var updated = tree.Disable(item.Id);

        Assert.That(updated.Items[0].IsEnabled, Is.False);
        Assert.That(tree.Items[0].IsEnabled, Is.True);
    }

    [Test]
    public void Enable_ShouldReturnTreeWithEnabledItem()
    {
        var item = CreateItem("Feature").WithEnabled(false);
        var tree = new LaserCad.Core.FeatureTree.FeatureTree(new[] { item });

        var updated = tree.Enable(item.Id);

        Assert.That(updated.Items[0].IsEnabled, Is.True);
        Assert.That(tree.Items[0].IsEnabled, Is.False);
    }

    [Test]
    public void Disable_WithUnknownItem_ShouldThrow()
    {
        var tree = new LaserCad.Core.FeatureTree.FeatureTree(new[] { CreateItem("Feature") });

        Assert.Throws<InvalidOperationException>(() => tree.Disable(Guid.NewGuid()));
    }

    private static FeatureTreeItem CreateItem(string name)
    {
        return new TestFeatureTreeItem(name: name);
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
