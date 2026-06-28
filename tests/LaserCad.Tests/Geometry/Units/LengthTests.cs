using LaserCad.Geometry;
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

    [Test]
    public void Multiply_ByNumber_ShouldReturnScaledLength()
    {
        var result = Length.FromCentimeters(2.5) * 3.0;

        Assert.That(result.Millimeters, Is.EqualTo(75.0));
    }

    [Test]
    public void Multiply_NumberByLength_ShouldReturnScaledLength()
    {
        var result = 3.0 * Length.FromCentimeters(2.5);

        Assert.That(result.Millimeters, Is.EqualTo(75.0));
    }

    [Test]
    public void Divide_ByNumber_ShouldReturnScaledLength()
    {
        var result = Length.FromMillimeters(75.0) / 3.0;

        Assert.That(result.Millimeters, Is.EqualTo(25.0));
    }

    [Test]
    public void Divide_ByZero_ShouldThrow()
    {
        var length = Length.FromMillimeters(75.0);

        Assert.Throws<DivideByZeroException>(() =>
        {
            _ = length / 0.0;
        });
    }

    [Test]
    public void IsApproximatelyEqualTo_ForDifferenceWithinDefaultTolerance_ShouldReturnTrue()
    {
        var length = Length.FromMillimeters(10.0);
        var other = Length.FromMillimeters(10.0 + GeometryTolerance.Default / 2.0);

        Assert.That(length.IsApproximatelyEqualTo(other), Is.True);
    }

    [Test]
    public void IsApproximatelyEqualTo_ForDifferenceAboveDefaultTolerance_ShouldReturnFalse()
    {
        var length = Length.FromMillimeters(10.0);
        var other = Length.FromMillimeters(10.0 + GeometryTolerance.Default * 2.0);

        Assert.That(length.IsApproximatelyEqualTo(other), Is.False);
    }

    [Test]
    public void IsApproximatelyEqualTo_WithNegativeTolerance_ShouldThrow()
    {
        var length = Length.FromMillimeters(10.0);
        var other = Length.FromMillimeters(10.0);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = length.IsApproximatelyEqualTo(other, -GeometryTolerance.Default);
        });
    }

    [Test]
    public void CompareTo_ForDifferenceWithinDefaultTolerance_ShouldReturnZero()
    {
        var length = Length.FromMillimeters(10.0);
        var other = Length.FromMillimeters(10.0 + GeometryTolerance.Default / 2.0);

        Assert.That(length.CompareTo(other), Is.EqualTo(0));
    }

    [Test]
    public void LessThan_ForDifferenceAboveDefaultTolerance_ShouldReturnTrue()
    {
        var length = Length.FromMillimeters(10.0);
        var other = Length.FromMillimeters(10.0 + GeometryTolerance.Default * 2.0);

        Assert.That(length < other, Is.True);
    }

    [Test]
    public void GreaterThan_ForDifferenceAboveDefaultTolerance_ShouldReturnTrue()
    {
        var length = Length.FromMillimeters(10.0 + GeometryTolerance.Default * 2.0);
        var other = Length.FromMillimeters(10.0);

        Assert.That(length > other, Is.True);
    }

    [Test]
    public void LessThanOrEqual_ForDifferenceWithinDefaultTolerance_ShouldReturnTrue()
    {
        var length = Length.FromMillimeters(10.0);
        var other = Length.FromMillimeters(10.0 + GeometryTolerance.Default / 2.0);

        Assert.That(length <= other, Is.True);
    }

    [Test]
    public void GreaterThanOrEqual_ForDifferenceWithinDefaultTolerance_ShouldReturnTrue()
    {
        var length = Length.FromMillimeters(10.0 + GeometryTolerance.Default / 2.0);
        var other = Length.FromMillimeters(10.0);

        Assert.That(length >= other, Is.True);
    }

    [Test]
    public void ToString_ShouldFormatValueInMillimeters()
    {
        var length = Length.FromMillimeters(12.5);

        Assert.That(length.ToString(), Is.EqualTo("12.5 mm"));
    }

    [Test]
    public void ToString_ShouldUseInvariantCultureByDefault()
    {
        var length = Length.FromMillimeters(12.5);

        Assert.That(length.ToString(), Does.Contain("."));
    }

    [Test]
    public void ToString_WithFormat_ShouldFormatValueInMillimeters()
    {
        var length = Length.FromMillimeters(12.3456);

        Assert.That(length.ToString("0.00", null), Is.EqualTo("12.35 mm"));
    }

    [Test]
    public void UnitConversions_FromMillimeters_ShouldKeepMillimeters()
    {
        var length = Length.FromMillimeters(123.456);

        Assert.That(length.Millimeters, Is.EqualTo(123.456));
    }

    [Test]
    public void UnitConversions_FromCentimeters_ShouldConvertToMillimeters()
    {
        var length = Length.FromCentimeters(12.3456);

        Assert.That(length.Millimeters, Is.EqualTo(123.456).Within(GeometryTolerance.Default));
    }

    [Test]
    public void UnitConversions_FromInches_ShouldConvertToMillimeters()
    {
        var length = Length.FromInches(4.86);

        Assert.That(length.Millimeters, Is.EqualTo(123.444).Within(GeometryTolerance.Default));
    }
}
