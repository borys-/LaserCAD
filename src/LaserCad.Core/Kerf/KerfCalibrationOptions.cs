using LaserCad.Geometry.Units;

namespace LaserCad.Core.Kerf;

/// <summary>
/// Opcje generatora probnika kalibracji kerfu.
/// </summary>
public sealed class KerfCalibrationOptions
{
    /// <summary>
    /// Tworzy opcje probnika kalibracyjnego.
    /// </summary>
    public KerfCalibrationOptions(
        Length? baseKerf = null,
        Length? kerfStep = null,
        int slotCount = 7,
        Length? slotWidth = null,
        Length? slotHeight = null,
        Length? spacing = null,
        Length? margin = null)
    {
        BaseKerf = EnsureNonNegative(baseKerf ?? Length.FromMillimeters(0.1), nameof(baseKerf));
        KerfStep = EnsurePositive(kerfStep ?? Length.FromMillimeters(0.02), nameof(kerfStep));
        SlotCount = slotCount;
        SlotWidth = EnsurePositive(slotWidth ?? Length.FromMillimeters(20.0), nameof(slotWidth));
        SlotHeight = EnsurePositive(slotHeight ?? Length.FromMillimeters(5.0), nameof(slotHeight));
        Spacing = EnsurePositive(spacing ?? Length.FromMillimeters(8.0), nameof(spacing));
        Margin = EnsurePositive(margin ?? Length.FromMillimeters(8.0), nameof(margin));

        if (SlotCount < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(slotCount), "Slot count must be at least 2.");
        }
    }

    /// <summary>
    /// Najmniejsza wartosc kerfu opisana na probniku.
    /// </summary>
    public Length BaseKerf { get; }

    /// <summary>
    /// Roznica miedzy kolejnymi wartosciami kerfu na probniku.
    /// </summary>
    public Length KerfStep { get; }

    /// <summary>
    /// Liczba szczelin testowych.
    /// </summary>
    public int SlotCount { get; }

    /// <summary>
    /// Nominalna szerokosc pojedynczej szczeliny.
    /// </summary>
    public Length SlotWidth { get; }

    /// <summary>
    /// Wysokosc pojedynczej szczeliny.
    /// </summary>
    public Length SlotHeight { get; }

    /// <summary>
    /// Odstep miedzy szczelinami.
    /// </summary>
    public Length Spacing { get; }

    /// <summary>
    /// Margines ramki probnika.
    /// </summary>
    public Length Margin { get; }

    /// <summary>
    /// Zwraca opisana wartosc kerfu dla szczeliny o podanym indeksie.
    /// </summary>
    public Length GetKerfAt(int index)
    {
        if (index < 0 || index >= SlotCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Slot index is outside calibration options.");
        }

        return Length.FromMillimeters(BaseKerf.Millimeters + (KerfStep.Millimeters * index));
    }

    private static Length EnsurePositive(Length value, string parameterName)
    {
        if (value.Millimeters <= 0.0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Length must be positive.");
        }

        return value;
    }

    private static Length EnsureNonNegative(Length value, string parameterName)
    {
        if (value.Millimeters < 0.0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Length must be non-negative.");
        }

        return value;
    }
}
