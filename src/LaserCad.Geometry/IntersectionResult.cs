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

    /// <summary>
    /// Rodzaj wyniku przeciecia.
    /// </summary>
    public IntersectionKind Kind { get; }
}
