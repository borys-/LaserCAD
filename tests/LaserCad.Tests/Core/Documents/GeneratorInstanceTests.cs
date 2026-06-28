using LaserCad.Core.Documents;
using LaserCad.Core.Parameters;

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

    [Test]
    public void Constructor_ShouldCreateEmptyParameterSet()
    {
        var generator = new GeneratorInstance();

        Assert.That(generator.Parameters.Parameters, Is.Empty);
    }

    [Test]
    public void AddParameter_ShouldReturnGeneratorWithAddedParameter()
    {
        var generator = new GeneratorInstance(generatorType: "Box");
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 100.0);

        var updatedGenerator = generator.AddParameter(parameter);

        Assert.That(updatedGenerator.Parameters.FindById(new ParameterId("Width")), Is.SameAs(parameter));
        Assert.That(updatedGenerator.Id, Is.EqualTo(generator.Id));
        Assert.That(updatedGenerator.GeneratorType, Is.EqualTo("Box"));
        Assert.That(generator.Parameters.Parameters, Is.Empty);
    }
}
