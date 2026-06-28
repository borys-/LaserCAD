using LaserCad.Core.Parameters;

namespace LaserCad.Tests.Core.Parameters;

public sealed class ParameterTests
{
    [Test]
    public void Constructor_ShouldStoreId()
    {
        var id = new ParameterId("Width");

        var parameter = new Parameter(id, ParameterType.Length);

        Assert.That(parameter.Id, Is.EqualTo(id));
    }

    [Test]
    public void Constructor_ShouldStoreType()
    {
        var parameter = new Parameter(new ParameterId("IsOpen"), ParameterType.Boolean);

        Assert.That(parameter.Type, Is.EqualTo(ParameterType.Boolean));
    }
}
