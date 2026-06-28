namespace LaserCad.Core.Documents;

/// <summary>
/// Okresla produkcyjna role warstwy dokumentu.
/// Wartosci roli sa rozwijane razem z kolejnymi ustawieniami eksportu.
/// </summary>
public enum LayerRole
{
    /// <summary>
    /// Warstwa przeznaczona do pelnego ciecia konturu.
    /// </summary>
    Cut,

    /// <summary>
    /// Warstwa przeznaczona do grawerowania powierzchniowego.
    /// </summary>
    Engrave,

    /// <summary>
    /// Warstwa przeznaczona do lekkiego nacinania albo znakowania linii.
    /// </summary>
    Score,

    /// <summary>
    /// Warstwa ignorowana przez eksport produkcyjny.
    /// </summary>
    Ignore
}
