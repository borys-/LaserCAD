using LaserCad.Geometry.Units;

namespace LaserCad.Core.Production;

/// <summary>
/// Rozmiar arkusza materialu uzywanego do przygotowania produkcji.
/// </summary>
public sealed class SheetSize
{
    /// <summary>
    /// Tworzy rozmiar arkusza.
    /// </summary>
    public SheetSize(Length width, Length height)
    {
        Width = EnsurePositive(width, nameof(width));
        Height = EnsurePositive(height, nameof(height));
    }

    /// <summary>
    /// Szerokosc arkusza.
    /// </summary>
    public Length Width { get; }

    /// <summary>
    /// Wysokosc arkusza.
    /// </summary>
    public Length Height { get; }

    private static Length EnsurePositive(Length value, string parameterName)
    {
        if (value <= Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(parameterName, "Sheet dimension must be greater than zero.");
        }

        return value;
    }
}
