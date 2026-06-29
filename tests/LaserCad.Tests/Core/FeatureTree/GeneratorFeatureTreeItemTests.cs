using LaserCad.Core.Documents;
using LaserCad.Core.FeatureTree;

namespace LaserCad.Tests.Core.FeatureTree;

public sealed class GeneratorFeatureTreeItemTests
{
    [Test]
    public void Constructor_ShouldUseGeneratorTypeAsDefaultName()
    {
        var generator = new GeneratorInstance(generatorType: "Box");

        var item = new GeneratorFeatureTreeItem(generator);

        Assert.That(item.Name, Is.EqualTo("Box"));
        Assert.That(item.Kind, Is.EqualTo(FeatureTreeItemKind.Generator));
        Assert.That(item.Generator, Is.SameAs(generator));
    }

    [Test]
    public void Apply_ShouldAddGeneratorAndGeneratedSketches()
    {
        var generator = new GeneratorInstance(generatorType: "Box");
        var sketch = new Sketch(name: "Generated");
        var item = new GeneratorFeatureTreeItem(generator, new[] { sketch });

        var rebuilt = item.Apply(new CadDocument());

        Assert.That(rebuilt.Generators, Has.Count.EqualTo(1));
        Assert.That(rebuilt.Generators[0], Is.SameAs(generator));
        Assert.That(rebuilt.Sketches, Has.Count.EqualTo(1));
        Assert.That(rebuilt.Sketches[0], Is.SameAs(sketch));
    }

    [Test]
    public void WithEnabled_ShouldPreserveGeneratorAndSketches()
    {
        var generator = new GeneratorInstance(generatorType: "Box");
        var sketch = new Sketch(name: "Generated");
        var item = new GeneratorFeatureTreeItem(generator, new[] { sketch });

        var disabled = (GeneratorFeatureTreeItem)item.WithEnabled(false);

        Assert.That(disabled.IsEnabled, Is.False);
        Assert.That(disabled.Generator, Is.SameAs(generator));
        Assert.That(disabled.GeneratedSketches[0], Is.SameAs(sketch));
    }
}
