namespace LaserCad.Core.Production;

/// <summary>
/// Oblicza proste statystyki produkcyjne dla ulozonego arkusza.
/// </summary>
public sealed class ProductionStatisticsCalculator
{
    private const double DefaultCuttingSpeedMillimetersPerMinute = 1000.0;

    /// <summary>
    /// Oblicza statystyki dla wyniku nestingu.
    /// </summary>
    public ProductionStatistics Calculate(
        SheetSize sheetSize,
        NestingResult result,
        double cuttingSpeedMillimetersPerMinute = DefaultCuttingSpeedMillimetersPerMinute)
    {
        if (sheetSize is null)
        {
            throw new ArgumentNullException(nameof(sheetSize));
        }

        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        if (cuttingSpeedMillimetersPerMinute <= 0.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cuttingSpeedMillimetersPerMinute),
                "Cutting speed must be greater than zero.");
        }

        var sheetArea = sheetSize.Width.Millimeters * sheetSize.Height.Millimeters;
        var partsArea = result.Parts.Sum(part => part.Width.Millimeters * part.Height.Millimeters);
        var cuttingLength = result.Parts.Sum(part => 2.0 * (part.Width.Millimeters + part.Height.Millimeters));
        var cuttingTime = cuttingLength / cuttingSpeedMillimetersPerMinute;

        return new ProductionStatistics(partsArea / sheetArea, cuttingLength, cuttingTime);
    }
}
