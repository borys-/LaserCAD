namespace LaserCad.Geometry;

/// <summary>
/// Luk okregu 2D.
/// Uzywaj go dla fragmentow okregu w szkicach i algorytmach geometrii.
/// </summary>
public readonly record struct Arc2D
{
    /// <summary>
    /// Tworzy luk o podanym srodku, promieniu oraz katach w radianach.
    /// </summary>
    public Arc2D(Point2D center, double radius, double startAngleRadians, double endAngleRadians)
    {
        if (radius <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be positive.");
        }

        Center = center;
        Radius = radius;
        StartAngleRadians = startAngleRadians;
        EndAngleRadians = endAngleRadians;
    }

    /// <summary>
    /// Srodek okregu, na ktorym lezy luk.
    /// </summary>
    public Point2D Center { get; }

    /// <summary>
    /// Promien luku w milimetrach domenowych.
    /// </summary>
    public double Radius { get; }

    /// <summary>
    /// Kat poczatkowy luku w radianach.
    /// </summary>
    public double StartAngleRadians { get; }

    /// <summary>
    /// Kat koncowy luku w radianach.
    /// </summary>
    public double EndAngleRadians { get; }
}
