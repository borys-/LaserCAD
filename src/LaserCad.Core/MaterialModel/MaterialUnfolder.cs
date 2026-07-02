using System.Globalization;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Rozwija materialowe elementy 3D do plaskich czesci przeznaczonych do nestingu i eksportu.
/// </summary>
public sealed class MaterialUnfolder
{
    /// <summary>
    /// Rozwija wszystkie plyty materialowe zapisane w dokumencie.
    /// </summary>
    public IReadOnlyList<FlatPart> Unfold(CadDocument document, MaterialUnfoldingOptions? options = null)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        var parts = document.MaterialSolids
            .Select(solid => FromMaterialSolid(solid, document.Layers))
            .Concat(document.SlopedMaterialSolids.SelectMany(solid => Unfold(solid, document.Layers)))
            .ToArray();

        return ApplyOptions(parts, options);
    }

    /// <summary>
    /// Rozwija pojedyncza plyte materialowa.
    /// </summary>
    public FlatPart Unfold(MaterialSolid solid, IEnumerable<Layer>? layers = null)
    {
        if (solid is null)
        {
            throw new ArgumentNullException(nameof(solid));
        }

        return FromMaterialSolid(solid, layers ?? DefaultLayers.All);
    }

    /// <summary>
    /// Rozwija kontrolowana bryle z pochyla sciana do szesciu plaskich czesci.
    /// </summary>
    public IReadOnlyList<FlatPart> Unfold(
        SlopedMaterialSolid solid,
        IEnumerable<Layer>? layers = null,
        MaterialUnfoldingOptions? options = null)
    {
        if (solid is null)
        {
            throw new ArgumentNullException(nameof(solid));
        }

        var effectiveLayers = layers?.ToArray() ?? DefaultLayers.All.ToArray();
        var parts = solid.Unfold()
            .Select(part => new FlatPart(
                $"{solid.Name} - {part.Name}",
                part.OuterContour,
                part.InnerContours,
                effectiveLayers,
                sourceNames: new[] { solid.Name, part.Name }))
            .ToArray();

        return ApplyOptions(parts, options);
    }

    private static FlatPart FromMaterialSolid(MaterialSolid solid, IEnumerable<Layer> layers)
    {
        var preview = solid.CreateFlatPreview();
        return new FlatPart(
            solid.Name,
            preview.OuterContour,
            preview.InnerContours,
            layers,
            sourceNames: new[] { solid.Name });
    }

    private static IReadOnlyList<FlatPart> ApplyOptions(
        IReadOnlyList<FlatPart> parts,
        MaterialUnfoldingOptions? options)
    {
        if (options?.MergeIdenticalParts != true)
        {
            return parts;
        }

        var merged = new List<FlatPart>();
        var byGeometry = new Dictionary<string, FlatPart>();

        foreach (var part in parts)
        {
            var key = CreateGeometryKey(part);
            if (!byGeometry.TryGetValue(key, out var existing))
            {
                byGeometry.Add(key, part);
                merged.Add(part);
                continue;
            }

            var replacement = existing.WithMergedSources(part.SourceNames);
            byGeometry[key] = replacement;
            merged[merged.IndexOf(existing)] = replacement;
        }

        return merged;
    }

    private static string CreateGeometryKey(FlatPart part)
    {
        var innerKeys = part.InnerContours
            .Select(CreatePolygonKey)
            .OrderBy(key => key, StringComparer.Ordinal)
            .ToArray();

        return string.Join("|", new[] { CreatePolygonKey(part.OuterContour) }.Concat(innerKeys));
    }

    private static string CreatePolygonKey(Polygon2D polygon)
    {
        var bounds = polygon.Bounds;
        var normalizedVertices = polygon.Vertices
            .Select(vertex => new Point2D(vertex.X - bounds.MinX, vertex.Y - bounds.MinY))
            .Select(point =>
                Math.Round(point.X, 6).ToString("0.######", CultureInfo.InvariantCulture)
                + ","
                + Math.Round(point.Y, 6).ToString("0.######", CultureInfo.InvariantCulture));

        return string.Join(";", normalizedVertices);
    }
}
