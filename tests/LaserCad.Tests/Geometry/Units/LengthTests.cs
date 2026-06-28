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

    [Test]
    public void FromInches_ShouldCreateEquivalentMillimeterLength()
    {
        var length = Length.FromInches(2.0);

        Assert.That(length, Is.EqualTo(Length.FromMillimeters(50.8)));
    }

    [Test]
    public void Millimeters_ShouldReturnMillimeterValue()
    {
        var length = Length.FromMillimeters(12.5);

        Assert.That(length.Millimeters, Is.EqualTo(12.5));
    }

    [Test]
    public void Millimeters_ShouldReturnConvertedCentimeterValue()
    {
        var length = Length.FromCentimeters(2.5);

        Assert.That(length.Millimeters, Is.EqualTo(25.0));
    }

    [Test]
    public void Millimeters_ShouldReturnConvertedInchValue()
    {
        var length = Length.FromInches(2.0);

        Assert.That(length.Millimeters, Is.EqualTo(50.8));
    }

    [Test]
    public void Add_ShouldReturnSumInMillimeters()
    {
        var result = Length.FromMillimeters(12.5) + Length.FromCentimeters(2.0);

        Assert.That(result.Millimeters, Is.EqualTo(32.5));
    }

    [Test]
    public void Subtract_ShouldReturnDifferenceInMillimeters()
    {
        var result = Length.FromInches(2.0) - Length.FromMillimeters(10.8);

        Assert.That(result.Millimeters, Is.EqualTo(40.0));
    }
}
