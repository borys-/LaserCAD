using LaserCad.Core.Parameters;

namespace LaserCad.Tests.Core.Parameters;

public sealed class ParameterTypeTests
{
    [Test]
    public void ParameterType_ShouldContainSupportedParameterKinds()
    {
        var values = Enum.GetValues<ParameterType>();

        Assert.That(values, Does.Contain(ParameterType.Length));
        Assert.That(values, Does.Contain(ParameterType.Number));
        Assert.That(values, Does.Contain(ParameterType.Boolean));
        Assert.That(values, Does.Contain(ParameterType.Text));
        Assert.That(values, Does.Contain(ParameterType.Choice));
    }
}
