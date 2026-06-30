using LaserCad.Geometry.Units;

namespace LaserCad.Core.Generators;

/// <summary>
/// Wspolne opcje prostych generatorow opartych o prostokatny obrys.
/// </summary>
public sealed class RectangularGeneratorOptions
{
    /// <summary>
    /// Tworzy opcje generatora prostokatnego.
    /// </summary>
    public RectangularGeneratorOptions(Length? width = null, Length? depth = null, Length? materialThickness = null)
    {
        Width = EnsurePositive(width ?? Length.FromMillimeters(120.0), nameof(width));
        Depth = EnsurePositive(depth ?? Length.FromMillimeters(80.0), nameof(depth));
        MaterialThickness = EnsurePositive(materialThickness ?? Length.FromMillimeters(3.0), nameof(materialThickness));
    }

    /// <summary>
    /// Szerokosc wyniku w milimetrach.
    /// </summary>
    public Length Width { get; }

    /// <summary>
    /// Glebokosc albo wysokosc rzutu generatora w milimetrach.
    /// </summary>
    public Length Depth { get; }

    /// <summary>
    /// Grubosc materialu uzywana dla detali konstrukcyjnych.
    /// </summary>
    public Length MaterialThickness { get; }

    private static Length EnsurePositive(Length value, string parameterName)
    {
        if (value <= Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(parameterName, "Generator dimension must be greater than zero.");
        }

        return value;
    }
}
