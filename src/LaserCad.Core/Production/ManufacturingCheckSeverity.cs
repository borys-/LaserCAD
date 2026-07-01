namespace LaserCad.Core.Production;

/// <summary>
/// Poziom istotnosci wyniku kontroli produkcyjnej.
/// </summary>
public enum ManufacturingCheckSeverity
{
    /// <summary>
    /// Informacja lub sugestia, ktora nie blokuje produkcji.
    /// </summary>
    Info,

    /// <summary>
    /// Ostrzezenie wymagajace uwagi przed produkcja.
    /// </summary>
    Warning,

    /// <summary>
    /// Blad, ktory powinien zostac poprawiony przed produkcja.
    /// </summary>
    Error,
}
