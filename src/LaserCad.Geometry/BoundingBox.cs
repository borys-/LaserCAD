namespace LaserCad.Geometry;

public readonly record struct BoundingBox(double MinX, double MinY, double MaxX, double MaxY)
{
    public double Width => MaxX - MinX;

    public double Height => MaxY - MinY;

    public static BoundingBox FromPoints(params Point2D[] points)
    {
        ArgumentNullException.ThrowIfNull(points);

        if (points.Length == 0)
        {
            throw new ArgumentException("At least one point is required.", nameof(points));
        }

        double minX = points[0].X;
        double minY = points[0].Y;
        double maxX = points[0].X;
        double maxY = points[0].Y;

        foreach (Point2D point in points[1..])
        {
            minX = Math.Min(minX, point.X);
            minY = Math.Min(minY, point.Y);
            maxX = Math.Max(maxX, point.X);
            maxY = Math.Max(maxY, point.Y);
        }

        return new BoundingBox(minX, minY, maxX, maxY);
    }

    public BoundingBox Union(BoundingBox other)
    {
        return new BoundingBox(
            Math.Min(MinX, other.MinX),
            Math.Min(MinY, other.MinY),
            Math.Max(MaxX, other.MaxX),
            Math.Max(MaxY, other.MaxY));
    }

    public bool Contains(Point2D point, double tolerance = GeometryTolerance.Default)
    {
        if (tolerance < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance cannot be negative.");
        }

        return point.X >= MinX - tolerance
            && point.X <= MaxX + tolerance
            && point.Y >= MinY - tolerance
            && point.Y <= MaxY + tolerance;
    }
}
