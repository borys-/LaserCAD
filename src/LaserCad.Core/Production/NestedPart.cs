using LaserCad.Geometry.Units;

namespace LaserCad.Core.Production;

/// <summary>
/// Element ulozony na arkuszu przez nesting.
/// </summary>
public sealed class NestedPart
{
    /// <summary>
    /// Tworzy wynik ulozenia elementu.
    /// </summary>
    public NestedPart(NestingItem item, Length x, Length y, Length width, Length height, bool isRotated)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
        X = x;
        Y = y;
        Width = width;
        Height = height;
        IsRotated = isRotated;
    }

    /// <summary>
    /// Element zrodlowy.
    /// </summary>
    public NestingItem Item { get; }

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
    /// Czy element zostal obrocony o 90 stopni.
    /// </summary>
    public bool IsRotated { get; }
}
