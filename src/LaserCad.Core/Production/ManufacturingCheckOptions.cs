using LaserCad.Geometry.Units;

namespace LaserCad.Core.Production;

/// <summary>
/// Opcje progow kontroli przygotowania dokumentu do produkcji.
/// </summary>
public sealed class ManufacturingCheckOptions
{
    /// <summary>
    /// Tworzy opcje kontroli produkcyjnych.
    /// </summary>
    public ManufacturingCheckOptions(Length? minimumSpacing = null, Length? minimumBridgeWidth = null)
    {
        MinimumSpacing = EnsureNonNegative(minimumSpacing ?? Length.FromMillimeters(1.0), nameof(minimumSpacing));
        MinimumBridgeWidth = EnsureNonNegative(minimumBridgeWidth ?? Length.FromMillimeters(2.0), nameof(minimumBridgeWidth));
    }

    /// <summary>
    /// Minimalny zalecany odstep miedzy niezaleznymi liniami ciecia.
    /// </summary>
    public Length MinimumSpacing { get; }

    /// <summary>
    /// Minimalna zalecana szerokosc mostka materialu.
    /// </summary>
    public Length MinimumBridgeWidth { get; }

    private static Length EnsureNonNegative(Length value, string parameterName)
    {
        if (value < Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(parameterName, "Manufacturing check threshold cannot be negative.");
        }

        return value;
    }
}
