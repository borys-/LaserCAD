using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Geometry.Units;

public sealed class LengthTests
{
    [Test]
    public void Length_ShouldBeValueType()
    {
        Assert.That(typeof(Length).IsValueType, Is.True);
    }
}
