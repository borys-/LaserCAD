namespace LaserCad.Geometry;

/// <summary>
/// Wynik operacji przeciecia dwoch obiektow geometrycznych.
/// </summary>
public readonly record struct IntersectionResult
{
    private IntersectionResult(IntersectionKind kind)
    {
        Kind = kind;
    }

    private IntersectionResult(IntersectionKind kind, Point2D? point)
        : this(kind)
    {
        Point = point;
    }

    /// <summary>
    /// Rodzaj wyniku przeciecia.
    /// </summary>
    public IntersectionKind Kind { get; }

    /// <summary>
    /// Punkt przeciecia, jesli wynik ma rodzaj <see cref="IntersectionKind.Point"/>.
    /// </summary>
    public Point2D? Point { get; }

    /// <summary>
    /// Zwraca wynik oznaczajacy brak przeciecia.
    /// </summary>
    public static IntersectionResult None()
    {
        return new IntersectionResult(IntersectionKind.None);
    }

    /// <summary>
    /// Okresla, czy wynik oznacza brak przeciecia.
    /// </summary>
    public bool IsNone => Kind == IntersectionKind.None;

    /// <summary>
    /// Zwraca wynik oznaczajacy pojedynczy punkt przeciecia.
    /// </summary>
    public static IntersectionResult FromPoint(Point2D point)
    {
        return new IntersectionResult(IntersectionKind.Point, point);
    }

    /// <summary>
    /// Okresla, czy wynik oznacza pojedynczy punkt przeciecia.
    /// </summary>
    public bool IsPoint => Kind == IntersectionKind.Point;

    /// <summary>
    /// Zwraca wynik oznaczajacy rownolegle, niewspolliniowe obiekty.
    /// </summary>
    public static IntersectionResult Parallel()
    {
        return new IntersectionResult(IntersectionKind.Parallel);
    }

    /// <summary>
    /// Okresla, czy wynik oznacza rownolegle, niewspolliniowe obiekty.
    /// </summary>
    public bool IsParallel => Kind == IntersectionKind.Parallel;
}
