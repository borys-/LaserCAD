using LaserCad.Geometry.Units;

namespace LaserCad.Core.Production;

/// <summary>
/// Opcje prostego nestingu elementow na arkuszu.
/// </summary>
public sealed class NestingOptions
{
    /// <summary>
    /// Tworzy opcje nestingu.
    /// </summary>
    public NestingOptions(Length? spacing = null)
    {
        Spacing = EnsureNonNegative(spacing ?? Length.FromMillimeters(2.0), nameof(spacing));
    }

    /// <summary>
    /// Odstep miedzy ulozonymi elementami.
    /// </summary>
    public Length Spacing { get; }

    private static Length EnsureNonNegative(Length value, string parameterName)
    {
        if (value < Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(parameterName, "Nesting spacing must be non-negative.");
        }

        return value;
    }
}
