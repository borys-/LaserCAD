namespace LaserCad.Geometry;

/// <summary>
/// Prostokat ograniczajacy w ukladzie 2D.
/// Uzywaj do szybkiego sprawdzania zasiegu encji, selekcji i eksportu.
/// </summary>
public readonly record struct BoundingBox(double MinX, double MinY, double MaxX, double MaxY)
{
    /// <summary>
    /// Szerokosc bounding boxa liczona jako MaxX - MinX.
    /// </summary>
    public double Width => MaxX - MinX;

    /// <summary>
    /// Wysokosc bounding boxa liczona jako MaxY - MinY.
    /// </summary>
    public double Height => MaxY - MinY;

    /// <summary>
    /// Tworzy bounding box obejmujacy podane punkty.
    /// Przekaz co najmniej jeden punkt.
    /// </summary>
    public static BoundingBox FromPoints(params Point2D[] points)
    {
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

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

    /// <summary>
    /// Zwraca bounding box obejmujacy ten i drugi bounding box.
    /// Uzywaj przy laczeniu zasiegow wielu encji.
    /// </summary>
    public BoundingBox Union(BoundingBox other)
    {
        return new BoundingBox(
            Math.Min(MinX, other.MinX),
            Math.Min(MinY, other.MinY),
            Math.Max(MaxX, other.MaxX),
            Math.Max(MaxY, other.MaxY));
    }

    /// <summary>
    /// Sprawdza, czy punkt miesci sie w bounding boxie z uwzglednieniem tolerancji.
    /// Uzywaj tolerancji dla stabilnych porownan zmiennoprzecinkowych.
    /// </summary>
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
