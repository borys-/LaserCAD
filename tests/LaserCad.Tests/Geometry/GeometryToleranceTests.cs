using LaserCad.Geometry;

namespace LaserCad.Tests.Geometry;

public sealed class GeometryToleranceTests
{
    [Test]
    public void Default_ShouldBePositive()
    {
        Assert.That(GeometryTolerance.Default, Is.GreaterThan(0.0));
    }

    [Test]
    public void Default_ShouldUseMicrometerToleranceInMillimeters()
    {
        Assert.That(GeometryTolerance.Default, Is.EqualTo(0.000001));
    }
}
