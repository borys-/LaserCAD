namespace LaserCad.Geometry;

/// <summary>
/// Odcinek 2D jako skonczony fragment prostej.
/// Uzywaj go dla podstawowych encji liniowych w szkicach i algorytmach geometrii.
/// </summary>
public readonly record struct LineSegment2D
{
    /// <summary>
    /// Tworzy zaczatek odcinka z punktem poczatkowym.
    /// </summary>
    public LineSegment2D(Point2D start)
    {
        Start = start;
    }

    /// <summary>
    /// Punkt poczatkowy odcinka.
    /// </summary>
    public Point2D Start { get; }
}
