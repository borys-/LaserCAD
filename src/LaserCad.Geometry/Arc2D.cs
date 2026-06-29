namespace LaserCad.Geometry;

/// <summary>
/// Luk okregu 2D.
/// Uzywaj go dla fragmentow okregu w szkicach i algorytmach geometrii.
/// </summary>
public readonly record struct Arc2D
{
    /// <summary>
    /// Tworzy luk o podanym srodku.
    /// </summary>
    public Arc2D(Point2D center)
    {
        Center = center;
    }

    /// <summary>
    /// Srodek okregu, na ktorym lezy luk.
    /// </summary>
    public Point2D Center { get; }
}
