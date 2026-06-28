namespace LaserCad.Geometry;

public readonly record struct Point2D(double X, double Y)
{
    public double DistanceTo(Point2D other)
    {
        return (this - other).Length;
    }

    public static Point2D operator +(Point2D point, Vector2D vector)
    {
        return new Point2D(point.X + vector.X, point.Y + vector.Y);
    }

    public static Point2D operator -(Point2D point, Vector2D vector)
    {
        return new Point2D(point.X - vector.X, point.Y - vector.Y);
    }

    public static Vector2D operator -(Point2D left, Point2D right)
    {
        return new Vector2D(left.X - right.X, left.Y - right.Y);
    }
}
