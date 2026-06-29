namespace LaserCad.Core.Dimensions;

/// <summary>
/// Okresla rodzaj wymiaru szkicu wspierany przez MVP.
/// </summary>
public enum DimensionKind
{
    /// <summary>
    /// Dlugosc odcinka liniowego.
    /// </summary>
    Length,

    /// <summary>
    /// Szerokosc prostokata.
    /// </summary>
    Width,

    /// <summary>
    /// Wysokosc prostokata.
    /// </summary>
    Height,

    /// <summary>
    /// Srednica okregu.
    /// </summary>
    Diameter,

    /// <summary>
    /// Promien okregu.
    /// </summary>
    Radius
}
