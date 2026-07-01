namespace LaserCad.Core.Production;

/// <summary>
/// Statystyki przygotowania produkcji.
/// </summary>
public sealed class ProductionStatistics
{
    /// <summary>
    /// Tworzy statystyki produkcyjne.
    /// </summary>
    public ProductionStatistics(
        double materialUsageRatio,
        double cuttingLengthMillimeters = 0.0,
        double estimatedCuttingTimeMinutes = 0.0)
    {
        if (materialUsageRatio < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(materialUsageRatio), "Material usage ratio must be non-negative.");
        }

        if (cuttingLengthMillimeters < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(cuttingLengthMillimeters), "Cutting length must be non-negative.");
        }

        if (estimatedCuttingTimeMinutes < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(estimatedCuttingTimeMinutes), "Cutting time must be non-negative.");
        }

        MaterialUsageRatio = materialUsageRatio;
        CuttingLengthMillimeters = cuttingLengthMillimeters;
        EstimatedCuttingTimeMinutes = estimatedCuttingTimeMinutes;
    }

    /// <summary>
    /// Udzial powierzchni elementow w powierzchni arkusza, w zakresie 0..1 dla poprawnego nestingu.
    /// </summary>
    public double MaterialUsageRatio { get; }

    /// <summary>
    /// Szacowana laczna dlugosc ciecia.
    /// </summary>
    public double CuttingLengthMillimeters { get; }

    /// <summary>
    /// Szacowany czas ciecia w minutach.
    /// </summary>
    public double EstimatedCuttingTimeMinutes { get; }
}
