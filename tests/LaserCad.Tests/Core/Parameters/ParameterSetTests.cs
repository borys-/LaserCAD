using LaserCad.Core.Parameters;

namespace LaserCad.Tests.Core.Parameters;

public sealed class ParameterSetTests
{
    [Test]
    public void Constructor_ShouldCreateEmptySet()
    {
        var parameterSet = new ParameterSet();

        Assert.That(parameterSet.Parameters, Is.Empty);
    }

    [Test]
    public void Constructor_WithParameters_ShouldStoreParameters()
    {
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 120.0);

        var parameterSet = new ParameterSet([parameter]);

        Assert.That(parameterSet.Parameters, Is.EqualTo(new[] { parameter }));
    }

    [Test]
    public void Constructor_WithNullParameters_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new ParameterSet(null!));
    }

    [Test]
    public void Constructor_WithNullParameter_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = new ParameterSet([null!]));
    }

    [Test]
    public void Add_ShouldReturnSetWithAddedParameter()
    {
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 120.0);
        var parameterSet = new ParameterSet();

        var updatedSet = parameterSet.Add(parameter);

        Assert.That(updatedSet.Parameters, Is.EqualTo(new[] { parameter }));
        Assert.That(parameterSet.Parameters, Is.Empty);
    }
}
