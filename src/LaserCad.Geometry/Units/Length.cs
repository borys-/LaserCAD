namespace LaserCad.Geometry.Units;

public readonly record struct Length
{
    private readonly double _millimeters;

    private Length(double millimeters)
    {
        _millimeters = millimeters;
    }

    public static Length FromMillimeters(double millimeters)
    {
        return new Length(millimeters);
    }
}
