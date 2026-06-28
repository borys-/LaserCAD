using LaserCad.Core.Documents;

namespace LaserCad.Tests.Core.Documents;

public sealed class GeneratorInstanceTests
{
    [Test]
    public void Constructor_ShouldCreateGeneratorInstance()
    {
        var generator = new GeneratorInstance();

        Assert.That(generator.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(generator.GeneratorType, Is.EqualTo("Generator"));
    }

    [Test]
    public void Constructor_WithIdAndType_ShouldStoreValues()
    {
        var id = Guid.NewGuid();

        var generator = new GeneratorInstance(id, "Box");

        Assert.That(generator.Id, Is.EqualTo(id));
        Assert.That(generator.GeneratorType, Is.EqualTo("Box"));
    }

    [Test]
    public void Constructor_WithEmptyType_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = new GeneratorInstance(generatorType: ""));
    }
}
