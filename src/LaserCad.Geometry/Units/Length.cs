namespace LaserCad.Geometry.Units;

public readonly record struct Length
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
}
