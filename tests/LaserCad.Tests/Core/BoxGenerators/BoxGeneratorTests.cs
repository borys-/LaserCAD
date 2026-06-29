using LaserCad.Core.BoxGenerators;
using LaserCad.Core.Documents;

namespace LaserCad.Tests.Core.BoxGenerators;

public sealed class BoxGeneratorTests
{
    [Test]
    public void GenerateSketch_ShouldCreateSketch()
    {
        var generator = new BoxGenerator();

        Sketch sketch = generator.GenerateSketch(new BoxGeneratorOptions());

        Assert.That(sketch.Name, Is.EqualTo("Box generator"));
    }
}
