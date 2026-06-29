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
}
