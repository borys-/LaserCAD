namespace LaserCad.Geometry.Units;

public readonly record struct Length
{
    private const double MillimetersPerCentimeter = 10.0;

    private readonly double _millimeters;

    private Length(double millimeters)
    {
        _millimeters = millimeters;
    }

    public static Length FromMillimeters(double millimeters)
    {
        return new Length(millimeters);
    }

    public static Length FromCentimeters(double centimeters)
    {
        return new Length(centimeters * MillimetersPerCentimeter);
    }
}
