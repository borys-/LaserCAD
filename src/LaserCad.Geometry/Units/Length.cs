using System.Globalization;
using LaserCad.Geometry;

namespace LaserCad.Geometry.Units;

public readonly record struct Length : IComparable<Length>, IFormattable
{
    private const double MillimetersPerCentimeter = 10.0;
    private const double MillimetersPerInch = 25.4;

    private readonly double _millimeters;

    private Length(double millimeters)
    {
        _millimeters = millimeters;
    }

    public double Millimeters => _millimeters;

    public static Length FromMillimeters(double millimeters)
    {
        return new Length(millimeters);
    }

    public static Length FromCentimeters(double centimeters)
    {
        return new Length(centimeters * MillimetersPerCentimeter);
    }

    public static Length FromInches(double inches)
    {
        return new Length(inches * MillimetersPerInch);
    }

    public bool IsApproximatelyEqualTo(Length other, double tolerance = GeometryTolerance.Default)
    {
        if (tolerance < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance cannot be negative.");
        }

        return Math.Abs(_millimeters - other._millimeters) <= tolerance;
    }

    public int CompareTo(Length other)
    {
        if (IsApproximatelyEqualTo(other))
        {
            return 0;
        }

        return _millimeters < other._millimeters ? -1 : 1;
    }

    public override string ToString()
    {
        return ToString("0.###", CultureInfo.InvariantCulture);
    }

    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var value = _millimeters.ToString(format ?? "0.###", formatProvider ?? CultureInfo.InvariantCulture);

        return $"{value} mm";
    }

    public static Length operator +(Length left, Length right)
    {
        return new Length(left._millimeters + right._millimeters);
    }

    public static Length operator -(Length left, Length right)
    {
        return new Length(left._millimeters - right._millimeters);
    }

    public static Length operator *(Length length, double multiplier)
    {
        return new Length(length._millimeters * multiplier);
    }

    public static Length operator *(double multiplier, Length length)
    {
        return length * multiplier;
    }

    public static Length operator /(Length length, double divisor)
    {
        if (divisor == 0.0)
        {
            throw new DivideByZeroException("Length cannot be divided by zero.");
        }

        return new Length(length._millimeters / divisor);
    }

    public static bool operator <(Length left, Length right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(Length left, Length right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(Length left, Length right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(Length left, Length right)
    {
        return left.CompareTo(right) >= 0;
    }
}
