using LaserCad.Core.Parameters;

namespace LaserCad.Tests.Core.Parameters;

public sealed class ParameterIdTests
{
    [Test]
    public void Constructor_WithValue_ShouldStoreValue()
    {
        var id = new ParameterId("Width");

        Assert.That(id.Value, Is.EqualTo("Width"));
    }

    [Test]
    public void Constructor_WithEmptyValue_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = new ParameterId(""));
    }

    [Test]
    public void ToString_ShouldReturnValue()
    {
        var id = new ParameterId("MaterialThickness");

        Assert.That(id.ToString(), Is.EqualTo("MaterialThickness"));
    }
}
