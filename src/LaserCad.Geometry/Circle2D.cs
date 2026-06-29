namespace LaserCad.Geometry;

/// <summary>
/// Okreg 2D.
/// Uzywaj go jako podstawowego kontraktu geometrycznego dla encji kolowych.
/// </summary>
public readonly record struct Circle2D
{
    /// <summary>
    /// Tworzy okreg o podanym srodku.
    /// </summary>
    public Circle2D(Point2D center)
    {
        Center = center;
    }

    /// <summary>
    /// Srodek okregu.
    /// </summary>
    public Point2D Center { get; }
}
