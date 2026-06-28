using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Geometry.Units;

public sealed class LengthTests
{
    [Test]
    public void Length_ShouldBeValueType()
    {
        Assert.That(typeof(Length).IsValueType, Is.True);
    }

    [Test]
    public void FromMillimeters_ShouldCreateEqualLengthsForEqualValues()
    {
        var length = Length.FromMillimeters(12.5);

        Assert.That(length, Is.EqualTo(Length.FromMillimeters(12.5)));
    }

    [Test]
    public void FromMillimeters_ShouldCreateDifferentLengthsForDifferentValues()
    {
        var length = Length.FromMillimeters(12.5);

        Assert.That(length, Is.Not.EqualTo(Length.FromMillimeters(10.0)));
    }

    [Test]
    public void FromCentimeters_ShouldCreateEquivalentMillimeterLength()
    {
        var length = Length.FromCentimeters(2.5);

        Assert.That(length, Is.EqualTo(Length.FromMillimeters(25.0)));
    }
}
