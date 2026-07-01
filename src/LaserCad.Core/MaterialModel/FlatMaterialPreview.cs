using LaserCad.Geometry;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Plaski podglad elementu materialowego z konturem zewnetrznym i otworami wewnetrznymi.
/// </summary>
public sealed class FlatMaterialPreview
{
    /// <summary>
    /// Tworzy plaski podglad produkcyjny.
    /// </summary>
    public FlatMaterialPreview(Polygon2D outerContour, IEnumerable<Polygon2D>? innerContours = null)
    {
        OuterContour = outerContour ?? throw new ArgumentNullException(nameof(outerContour));
        InnerContours = innerContours?.ToArray() ?? Array.Empty<Polygon2D>();
    }

    /// <summary>
    /// Kontur zewnetrzny czesci.
    /// </summary>
    public Polygon2D OuterContour { get; }

    /// <summary>
    /// Kontury wewnetrzne wyciec.
    /// </summary>
    public IReadOnlyList<Polygon2D> InnerContours { get; }
}
