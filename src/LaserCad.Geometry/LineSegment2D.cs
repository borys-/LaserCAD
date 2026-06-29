namespace LaserCad.Geometry;

/// <summary>
/// Odcinek 2D jako skonczony fragment prostej.
/// Uzywaj go dla podstawowych encji liniowych w szkicach i algorytmach geometrii.
/// </summary>
public readonly record struct LineSegment2D
{
    /// <summary>
    /// Tworzy odcinek pomiedzy punktem poczatkowym i koncowym.
    /// </summary>
    public LineSegment2D(Point2D start, Point2D end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// Punkt poczatkowy odcinka.
    /// </summary>
    public Point2D Start { get; }

    /// <summary>
    /// Punkt koncowy odcinka.
    /// </summary>
    public Point2D End { get; }

    /// <summary>
    /// Dlugosc odcinka w milimetrach domenowych.
    /// </summary>
    public double Length => Start.DistanceTo(End);

    /// <summary>
    /// Znormalizowany kierunek od poczatku do konca odcinka.
    /// Nie uzywaj dla odcinka zerowej dlugosci, bo metoda zglosi blad.
    /// </summary>
    public Vector2D Direction => (End - Start).Normalize();

    /// <summary>
    /// Bounding box obejmujacy punkt poczatkowy i koncowy odcinka.
    /// </summary>
    public BoundingBox Bounds => BoundingBox.FromPoints(Start, End);

    /// <summary>
    /// Zwraca punkt posredni dla parametru t z zakresu od 0 do 1.
    /// Dla t = 0 zwraca poczatek, dla t = 1 zwraca koniec.
    /// </summary>
    public Point2D PointAt(double t)
    {
        if (t < 0.0 || t > 1.0)
        {
            throw new ArgumentOutOfRangeException(nameof(t), "Parameter t must be between 0 and 1.");
        }

        return new Point2D(
            Start.X + ((End.X - Start.X) * t),
            Start.Y + ((End.Y - Start.Y) * t));
    }

    /// <summary>
    /// Zwraca odcinek po zastosowaniu transformacji afinicznej.
    /// </summary>
    public LineSegment2D Transform(Matrix3x3 transform)
    {
        return new LineSegment2D(
            transform.Transform(Start),
            transform.Transform(End));
    }
}
