namespace LaserCad.Geometry;

/// <summary>
/// Orientacja wierzcholkow polygonu w ukladzie 2D.
/// </summary>
public enum PolygonOrientation
{
    /// <summary>
    /// Wierzcholki sa ulozone zgodnie z ruchem wskazowek zegara.
    /// </summary>
    Clockwise,

    /// <summary>
    /// Wierzcholki sa ulozone przeciwnie do ruchu wskazowek zegara.
    /// </summary>
    Counterclockwise,
}
