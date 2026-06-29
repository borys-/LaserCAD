namespace LaserCad.Geometry;

/// <summary>
/// Okreg 2D.
/// Uzywaj go jako podstawowego kontraktu geometrycznego dla encji kolowych.
/// </summary>
public readonly record struct Circle2D
{
    /// <summary>
    /// Tworzy okreg o podanym srodku i promieniu.
    /// </summary>
    public Circle2D(Point2D center, double radius)
    {
        if (radius <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be positive.");
        }

        Center = center;
        Radius = radius;
    }

    /// <summary>
    /// Srodek okregu.
    /// </summary>
    public Point2D Center { get; }

    /// <summary>
    /// Promien okregu w milimetrach domenowych.
    /// </summary>
    public double Radius { get; }

    /// <summary>
    /// Obwod okregu w milimetrach domenowych.
    /// </summary>
    public double Circumference => 2.0 * Math.PI * Radius;

    /// <summary>
    /// Bounding box obejmujacy caly okreg.
    /// </summary>
    public BoundingBox Bounds => new BoundingBox(
        Center.X - Radius,
        Center.Y - Radius,
        Center.X + Radius,
        Center.Y + Radius);
}
