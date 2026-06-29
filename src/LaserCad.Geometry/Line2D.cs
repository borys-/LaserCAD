namespace LaserCad.Geometry;

/// <summary>
/// Nieskonczona linia 2D opisana punktem i kierunkiem.
/// Uzywaj jej jako kontraktu pomocniczego dla przeciec i rzutowan.
/// </summary>
public readonly record struct Line2D
{
    /// <summary>
    /// Tworzy linie przechodzaca przez podany punkt w podanym kierunku.
    /// Kierunek jest normalizowany; wektor zerowy zglosi blad.
    /// </summary>
    public Line2D(Point2D point, Vector2D direction)
    {
        Point = point;
        Direction = direction.Normalize();
    }

    /// <summary>
    /// Dowolny punkt lezacy na linii.
    /// </summary>
    public Point2D Point { get; }

    /// <summary>
    /// Znormalizowany kierunek linii.
    /// </summary>
    public Vector2D Direction { get; }
}
