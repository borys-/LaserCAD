namespace LaserCad.Core.Documents;

/// <summary>
/// Katalog standardowych warstw tworzonych dla nowego dokumentu.
/// Uzywaj ich jako punktu startowego dla typowego projektu laserowego.
/// </summary>
public static class DefaultLayers
{
    /// <summary>
    /// Domyslna warstwa ciecia.
    /// </summary>
    public static Layer Cut { get; } = new("Cut", LayerColor.FromHex("#FF0000"), LayerRole.Cut);

    /// <summary>
    /// Domyslna warstwa grawerowania.
    /// </summary>
    public static Layer Engrave { get; } = new("Engrave", LayerColor.FromHex("#0000FF"), LayerRole.Engrave);

    /// <summary>
    /// Domyslna warstwa nacinania.
    /// </summary>
    public static Layer Score { get; } = new("Score", LayerColor.FromHex("#00AA00"), LayerRole.Score);

    /// <summary>
    /// Domyslna warstwa pomocnicza ignorowana przez eksport produkcyjny.
    /// </summary>
    public static Layer Ignore { get; } = new("Ignore", LayerColor.FromHex("#808080"), LayerRole.Ignore);

    /// <summary>
    /// Pelna lista domyslnych warstw w kolejnosci prezentacji.
    /// </summary>
    public static IReadOnlyList<Layer> All { get; } = new[] { Cut, Engrave, Score, Ignore };
}
