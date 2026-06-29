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
}
