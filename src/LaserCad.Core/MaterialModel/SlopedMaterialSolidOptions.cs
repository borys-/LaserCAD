using LaserCad.Geometry.Units;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Parametry bryly materialowej z jedna pochyla sciana.
/// </summary>
public sealed class SlopedMaterialSolidOptions
{
    /// <summary>
    /// Tworzy zestaw wymiarow bryly z pochyleniem miedzy przednia i tylna wysokoscia.
    /// </summary>
    public SlopedMaterialSolidOptions(
        Length width,
        Length depth,
        Length frontHeight,
        Length backHeight)
    {
        EnsurePositive(width, nameof(width));
        EnsurePositive(depth, nameof(depth));
        EnsurePositive(frontHeight, nameof(frontHeight));
        EnsurePositive(backHeight, nameof(backHeight));

        Width = width;
        Depth = depth;
        FrontHeight = frontHeight;
        BackHeight = backHeight;
    }

    /// <summary>
    /// Szerokosc bryly.
    /// </summary>
    public Length Width { get; }

    /// <summary>
    /// Glebokosc bryly.
    /// </summary>
    public Length Depth { get; }

    /// <summary>
    /// Wysokosc przedniej sciany.
    /// </summary>
    public Length FrontHeight { get; }

    /// <summary>
    /// Wysokosc tylnej sciany.
    /// </summary>
    public Length BackHeight { get; }

    /// <summary>
    /// Roznica wysokosci wyznaczajaca pochylenie gornej sciany.
    /// </summary>
    public Length HeightDelta => Length.FromMillimeters(BackHeight.Millimeters - FrontHeight.Millimeters);

    /// <summary>
    /// Dlugosc pochylej gornej krawedzi w przekroju bocznym.
    /// </summary>
    public Length SlopedDepth => Length.FromMillimeters(
        Math.Sqrt(
            (Depth.Millimeters * Depth.Millimeters)
            + (HeightDelta.Millimeters * HeightDelta.Millimeters)));

    private static void EnsurePositive(Length length, string parameterName)
    {
        if (length.Millimeters <= 0.0)
        {
            throw new ArgumentOutOfRangeException(parameterName, "Sloped material solid dimensions must be positive.");
        }
    }
}
