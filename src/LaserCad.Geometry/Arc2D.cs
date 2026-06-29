namespace LaserCad.Geometry;

/// <summary>
/// Luk okregu 2D.
/// Uzywaj go dla fragmentow okregu w szkicach i algorytmach geometrii.
/// </summary>
public readonly record struct Arc2D
{
    private const double FullTurnRadians = Math.PI * 2.0;

    /// <summary>
    /// Tworzy luk o podanym srodku, promieniu oraz katach w radianach.
    /// Domyslny kierunek luku jest przeciwny do ruchu wskazowek zegara.
    /// </summary>
    public Arc2D(
        Point2D center,
        double radius,
        double startAngleRadians,
        double endAngleRadians,
        ArcDirection direction = ArcDirection.Counterclockwise)
    {
        if (radius <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be positive.");
        }

        Center = center;
        Radius = radius;
        StartAngleRadians = startAngleRadians;
        EndAngleRadians = endAngleRadians;
        Direction = direction;
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

    /// <summary>
    /// Kierunek przejscia po luku od kata poczatkowego do koncowego.
    /// </summary>
    public ArcDirection Direction { get; }

    /// <summary>
    /// Dlugosc luku w milimetrach domenowych.
    /// </summary>
    public double Length => Radius * SweepAngleRadians;

    private double SweepAngleRadians
    {
        get
        {
            double delta = Direction == ArcDirection.Counterclockwise
                ? EndAngleRadians - StartAngleRadians
                : StartAngleRadians - EndAngleRadians;

            return NormalizePositiveAngle(delta);
        }
    }

    private static double NormalizePositiveAngle(double angleRadians)
    {
        double result = angleRadians % FullTurnRadians;

        if (result < 0.0)
        {
            result += FullTurnRadians;
        }

        return result;
    }
}
