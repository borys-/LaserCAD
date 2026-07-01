namespace LaserCad.Core.Production;

/// <summary>
/// Statystyki przygotowania produkcji.
/// </summary>
public sealed class ProductionStatistics
{
    /// <summary>
    /// Tworzy statystyki produkcyjne.
    /// </summary>
    public ProductionStatistics(double materialUsageRatio)
    {
        if (materialUsageRatio < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(materialUsageRatio), "Material usage ratio must be non-negative.");
        }

        MaterialUsageRatio = materialUsageRatio;
    }

    /// <summary>
    /// Udzial powierzchni elementow w powierzchni arkusza, w zakresie 0..1 dla poprawnego nestingu.
    /// </summary>
    public double MaterialUsageRatio { get; }
}
