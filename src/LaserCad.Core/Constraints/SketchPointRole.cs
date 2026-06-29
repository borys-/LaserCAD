namespace LaserCad.Core.Constraints;

/// <summary>
/// Rola punktu referencyjnego encji szkicu.
/// MVP obsluguje punkty poczatku i konca linii.
/// </summary>
public enum SketchPointRole
{
    /// <summary>
    /// Punkt poczatkowy encji liniowej.
    /// </summary>
    Start,

    /// <summary>
    /// Punkt koncowy encji liniowej.
    /// </summary>
    End,
}
