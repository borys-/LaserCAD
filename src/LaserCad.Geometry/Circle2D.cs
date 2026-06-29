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

    /// <summary>
    /// Zwraca okreg po zastosowaniu transformacji afinicznej zachowujacej ksztalt okregu.
    /// Transformacja moze przesuwac, obracac, odbijac i skalowac jednolicie.
    /// </summary>
    public Circle2D Transform(Matrix3x3 transform)
    {
        Point2D transformedCenter = transform.Transform(Center);
        double transformedRadius = GetTransformedRadius(transform, Radius);

        return new Circle2D(transformedCenter, transformedRadius);
    }

    internal static double GetTransformedRadius(Matrix3x3 transform, double radius)
    {
        Vector2D transformedX = transform.Transform(new Vector2D(radius, 0.0));
        Vector2D transformedY = transform.Transform(new Vector2D(0.0, radius));

        if (Math.Abs(transformedX.Length - transformedY.Length) > GeometryTolerance.Default
            || Math.Abs(transformedX.Dot(transformedY)) > GeometryTolerance.Default)
        {
            throw new InvalidOperationException("Transform must preserve circular geometry.");
        }

        return transformedX.Length;
    }
}
