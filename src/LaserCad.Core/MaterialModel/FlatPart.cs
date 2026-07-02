using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Plaska czesc produkcyjna powstala po rozwinieciu elementu materialowego 3D.
/// </summary>
public sealed class FlatPart
{
    /// <summary>
    /// Tworzy plaska czesc z konturem zewnetrznym, otworami i warstwami technologicznymi.
    /// </summary>
    public FlatPart(
        string name,
        Polygon2D outerContour,
        IEnumerable<Polygon2D>? innerContours = null,
        IEnumerable<Layer>? layers = null,
        int quantity = 1,
        IEnumerable<string>? sourceNames = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Flat part name cannot be empty.", nameof(name));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Flat part quantity must be greater than zero.");
        }

        Name = name;
        OuterContour = outerContour ?? throw new ArgumentNullException(nameof(outerContour));
        InnerContours = innerContours?.ToArray() ?? Array.Empty<Polygon2D>();
        Layers = layers?.ToArray() ?? DefaultLayers.All.ToArray();
        Quantity = quantity;
        SourceNames = sourceNames?.ToArray() ?? new[] { name };

        if (Layers.Any(layer => layer is null))
        {
            throw new ArgumentException("Flat part layers cannot contain null values.", nameof(layers));
        }

        if (SourceNames.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException("Flat part source names cannot be empty.", nameof(sourceNames));
        }
    }

    /// <summary>
    /// Etykieta czesci pokazywana uzytkownikowi i uzywana w przyszlym eksporcie.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Zewnetrzny kontur ciecia czesci.
    /// </summary>
    public Polygon2D OuterContour { get; }

    /// <summary>
    /// Wewnetrzne kontury ciecia, np. otwory po negatywach.
    /// </summary>
    public IReadOnlyList<Polygon2D> InnerContours { get; }

    /// <summary>
    /// Warstwy technologiczne przeniesione z dokumentu do przygotowania produkcyjnego.
    /// </summary>
    public IReadOnlyList<Layer> Layers { get; }

    /// <summary>
    /// Liczba identycznych czesci po deduplikacji geometrii.
    /// </summary>
    public int Quantity { get; }

    /// <summary>
    /// Nazwy elementow zrodlowych, z ktorych powstala ta czesc.
    /// </summary>
    public IReadOnlyList<string> SourceNames { get; }

    /// <summary>
    /// Zwraca czesc z powiekszona liczba wystapien i dopisanymi nazwami zrodlowymi.
    /// </summary>
    public FlatPart WithMergedSources(IEnumerable<string> sourceNames)
    {
        if (sourceNames is null)
        {
            throw new ArgumentNullException(nameof(sourceNames));
        }

        var mergedSourceNames = SourceNames.Concat(sourceNames).ToArray();
        return new FlatPart(Name, OuterContour, InnerContours, Layers, Quantity + sourceNames.Count(), mergedSourceNames);
    }
}
