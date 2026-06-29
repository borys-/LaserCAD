namespace LaserCad.Core.BoxGenerators;

/// <summary>
/// Okresla wariant pudelka tworzony przez generator.
/// </summary>
public enum BoxGeneratorType
{
    /// <summary>
    /// Pudelko zamkniete, z kompletem scianek.
    /// </summary>
    Closed,

    /// <summary>
    /// Pudelko otwarte, bez pokrywy/gornej scianki.
    /// </summary>
    Open,

    /// <summary>
    /// Pudelko z osobna pokrywa.
    /// </summary>
    WithLid
}
