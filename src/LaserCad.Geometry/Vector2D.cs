namespace LaserCad.Geometry;

public readonly record struct Vector2D(double X, double Y)
{
    public double Length => Math.Sqrt((X * X) + (Y * Y));

    public Vector2D Normalize()
    {
        double length = Length;

        if (length <= GeometryTolerance.Default)
        {
            throw new InvalidOperationException("Zero length vector cannot be normalized.");
        }

        return new Vector2D(X / length, Y / length);
    }

    public double Dot(Vector2D other)
    {
        return (X * other.X) + (Y * other.Y);
    }

    public double Cross(Vector2D other)
    {
        return (X * other.Y) - (Y * other.X);
    }

    public double AngleTo(Vector2D other)
    {
        Vector2D left = Normalize();
        Vector2D right = other.Normalize();
        double dot = Math.Clamp(left.Dot(right), -1.0, 1.0);

        return Math.Acos(dot);
    }
}
