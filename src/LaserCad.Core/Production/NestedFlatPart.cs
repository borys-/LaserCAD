using LaserCad.Core.MaterialModel;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.Production;

/// <summary>
/// Plaska czesc ulozona na arkuszu materialu.
/// </summary>
public sealed class NestedFlatPart
{
    /// <summary>
    /// Tworzy ulozona czesc plaska.
    /// </summary>
    public NestedFlatPart(FlatPart part, Length x, Length y, Length width, Length height, bool isRotated)
    {
        Part = part ?? throw new ArgumentNullException(nameof(part));
        X = x;
        Y = y;
        Width = width;
        Height = height;
        IsRotated = isRotated;
    }

    /// <summary>
    /// Czesciowy model produkcyjny przed ulozeniem.
    /// </summary>
    public FlatPart Part { get; }

    /// <summary>
    /// Polozenie lewego dolnego rogu na arkuszu.
    /// </summary>
    public Length X { get; }

    /// <summary>
    /// Polozenie lewego dolnego rogu na arkuszu.
    /// </summary>
    public Length Y { get; }

    /// <summary>
    /// Szerokosc po ewentualnym obrocie.
    /// </summary>
    public Length Width { get; }

    /// <summary>
    /// Wysokosc po ewentualnym obrocie.
    /// </summary>
    public Length Height { get; }

    /// <summary>
    /// Czy czesc zostala obrocona o 90 stopni.
    /// </summary>
    public bool IsRotated { get; }
}
