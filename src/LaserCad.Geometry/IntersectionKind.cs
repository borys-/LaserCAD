namespace LaserCad.Geometry;

/// <summary>
/// Rodzaj wyniku przeciecia dwoch obiektow geometrycznych.
/// </summary>
public enum IntersectionKind
{
    /// <summary>
    /// Obiekty nie maja wspolnych punktow.
    /// </summary>
    None,

    /// <summary>
    /// Obiekty przecinaja sie w jednym punkcie.
    /// </summary>
    Point,

    /// <summary>
    /// Obiekty sa rownolegle i nie leza na tej samej prostej.
    /// </summary>
    Parallel,

    /// <summary>
    /// Obiekty leza na tej samej prostej i moga miec wspolny zakres.
    /// </summary>
    Overlap,
}
