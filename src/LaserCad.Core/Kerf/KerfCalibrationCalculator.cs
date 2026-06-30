using LaserCad.Geometry.Units;

namespace LaserCad.Core.Kerf;

/// <summary>
/// Przelicza pomiar probnika kerfu na rekomendowana wartosc kerfu.
/// </summary>
public static class KerfCalibrationCalculator
{
    /// <summary>
    /// Oblicza rekomendowany kerf z wybranej szczeliny probnika i jej zmierzonej szerokosci.
    /// </summary>
    public static Length CalculateRecommendedKerf(
        KerfCalibrationOptions options,
        int slotIndex,
        Length measuredSlotWidth)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (measuredSlotWidth.Millimeters <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(measuredSlotWidth), "Measured slot width must be positive.");
        }

        var labeledKerf = options.GetKerfAt(slotIndex);
        var widthError = options.SlotWidth.Millimeters - measuredSlotWidth.Millimeters;
        var recommendedKerf = labeledKerf.Millimeters + widthError;

        if (recommendedKerf < 0.0)
        {
            recommendedKerf = 0.0;
        }

        return Length.FromMillimeters(recommendedKerf);
    }
}
