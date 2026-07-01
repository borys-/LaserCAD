namespace LaserCad.Core.Production;

/// <summary>
/// Oblicza proste statystyki produkcyjne dla ulozonego arkusza.
/// </summary>
public sealed class ProductionStatisticsCalculator
{
    /// <summary>
    /// Oblicza statystyki dla wyniku nestingu.
    /// </summary>
    public ProductionStatistics Calculate(SheetSize sheetSize, NestingResult result)
    {
        if (sheetSize is null)
        {
            throw new ArgumentNullException(nameof(sheetSize));
        }

        if (result is null)
        {
            throw new ArgumentNullException(nameof(result));
        }

        var sheetArea = sheetSize.Width.Millimeters * sheetSize.Height.Millimeters;
        var partsArea = result.Parts.Sum(part => part.Width.Millimeters * part.Height.Millimeters);
        var cuttingLength = result.Parts.Sum(part => 2.0 * (part.Width.Millimeters + part.Height.Millimeters));

        return new ProductionStatistics(partsArea / sheetArea, cuttingLength);
    }
}
