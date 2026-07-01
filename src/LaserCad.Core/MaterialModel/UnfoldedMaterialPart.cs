using LaserCad.Geometry;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Plaska czesc materialowa powstala po rozwinieciu bryly 3D.
/// </summary>
public sealed class UnfoldedMaterialPart
{
    /// <summary>
    /// Tworzy plaska czesc z nazwa i konturem zewnetrznym.
    /// </summary>
    public UnfoldedMaterialPart(string name, Polygon2D outerContour, IEnumerable<Polygon2D>? innerContours = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Unfolded material part name cannot be empty.", nameof(name));
        }

        Name = name;
        OuterContour = outerContour ?? throw new ArgumentNullException(nameof(outerContour));
        InnerContours = innerContours?.ToArray() ?? Array.Empty<Polygon2D>();
    }

    /// <summary>
    /// Nazwa czesci wynikowej.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Zewnetrzny kontur czesci do wyciecia.
    /// </summary>
    public Polygon2D OuterContour { get; }

    /// <summary>
    /// Kontury wewnetrzne wyciec nalezacych do czesci.
    /// </summary>
    public IReadOnlyList<Polygon2D> InnerContours { get; }
}
