using LaserCad.Core.Parameters;

namespace LaserCad.Tests.Core.Parameters;

public sealed class ParameterTests
{
    [Test]
    public void Constructor_ShouldStoreId()
    {
        var id = new ParameterId("Width");

        var parameter = new Parameter(id, "Width", ParameterType.Length, 120.0);

        Assert.That(parameter.Id, Is.EqualTo(id));
    }

    [Test]
    public void Constructor_ShouldStoreType()
    {
        var parameter = new Parameter(new ParameterId("IsOpen"), "Is open", ParameterType.Boolean, true);

        Assert.That(parameter.Type, Is.EqualTo(ParameterType.Boolean));
    }

    [Test]
    public void Constructor_ShouldStoreName()
    {
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Length, 120.0);

        Assert.That(parameter.Name, Is.EqualTo("Width"));
    }

    [Test]
    public void Constructor_WithEmptyName_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = new Parameter(new ParameterId("Width"), "", ParameterType.Length, 120.0));
    }

    [Test]
    public void Constructor_ShouldStoreValue()
    {
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 120.0);

        Assert.That(parameter.Value, Is.EqualTo(120.0));
    }
}
