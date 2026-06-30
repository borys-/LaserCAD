namespace LaserCad.Core.Kerf;

/// <summary>
/// Tryb kompensacji kerfu wzgledem nominalnego konturu.
/// </summary>
public enum KerfCompensationMode
{
    /// <summary>
    /// Offset odsuwa zewnetrzne kontury na zewnatrz, a otwory do wewnatrz.
    /// </summary>
    Positive,

    /// <summary>
    /// Offset przesuwa zewnetrzne kontury do wewnatrz, a otwory na zewnatrz.
    /// </summary>
    Negative,
}
