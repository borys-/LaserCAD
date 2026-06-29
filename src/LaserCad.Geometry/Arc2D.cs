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

    /// <summary>
    /// Zwraca punkt na luku dla parametru t z zakresu od 0 do 1.
    /// Dla t = 0 zwraca poczatek luku, dla t = 1 zwraca koniec luku.
    /// </summary>
    public Point2D PointAt(double t)
    {
        if (t < 0.0 || t > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(t), "Parameter t must be between 0 and 1.");
        }

        double signedSweep = Direction == ArcDirection.Counterclockwise
            ? SweepAngleRadians
            : -SweepAngleRadians;
        double angle = StartAngleRadians + (signedSweep * t);

        return PointAtAngle(angle);
    }

    /// <summary>
    /// Zwraca luk po zastosowaniu transformacji afinicznej zachowujacej ksztalt okregu.
    /// Transformacja moze przesuwac, obracac, odbijac i skalowac jednolicie.
    /// </summary>
    public Arc2D Transform(Matrix3x3 transform)
    {
        Point2D transformedCenter = transform.Transform(Center);
        double transformedRadius = Circle2D.GetTransformedRadius(transform, Radius);
        Point2D transformedStart = transform.Transform(PointAt(0.0));
        Point2D transformedEnd = transform.Transform(PointAt(1.0));
        ArcDirection transformedDirection = IsOrientationReversed(transform)
            ? Reverse(Direction)
            : Direction;

        return new Arc2D(
            transformedCenter,
            transformedRadius,
            AngleFromCenter(transformedCenter, transformedStart),
            AngleFromCenter(transformedCenter, transformedEnd),
            transformedDirection);
    }

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

    private Point2D PointAtAngle(double angleRadians)
    {
        return new Point2D(
            Center.X + (Math.Cos(angleRadians) * Radius),
            Center.Y + (Math.Sin(angleRadians) * Radius));
    }

    private static double AngleFromCenter(Point2D center, Point2D point)
    {
        return Math.Atan2(point.Y - center.Y, point.X - center.X);
    }

    private static bool IsOrientationReversed(Matrix3x3 transform)
    {
        double determinant = (transform.M11 * transform.M22) - (transform.M12 * transform.M21);

        return determinant < 0.0;
    }

    private static ArcDirection Reverse(ArcDirection direction)
    {
        return direction == ArcDirection.Counterclockwise
            ? ArcDirection.Clockwise
            : ArcDirection.Counterclockwise;
    }
}
