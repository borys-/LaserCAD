namespace LaserCad.Core.Kerf;

/// <summary>
/// Klasyfikacja konturu uzywana przy kompensacji kerfu.
/// </summary>
public enum KerfContourClassification
{
    /// <summary>
    /// Kontur zewnetrzny elementu.
    /// </summary>
    Outer,

    /// <summary>
    /// Kontur wewnetrzny, np. otwor.
    /// </summary>
    Inner,
}
