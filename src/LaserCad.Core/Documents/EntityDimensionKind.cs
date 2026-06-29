namespace LaserCad.Core.Documents;

/// <summary>
/// Okresla wymiar encji szkicu, ktory moze byc sterowany parametrem.
/// MVP wspiera szerokosc, wysokosc i srednice.
/// </summary>
public enum EntityDimensionKind
{
    /// <summary>
    /// Szerokosc encji, np. prostokata osiowego.
    /// </summary>
    Width,

    /// <summary>
    /// Wysokosc encji, np. prostokata osiowego.
    /// </summary>
    Height,

    /// <summary>
    /// Srednica encji kolowej.
    /// </summary>
    Diameter
}
