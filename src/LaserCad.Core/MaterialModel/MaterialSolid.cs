using LaserCad.Core.Documents;
using LaserCad.Core.Preview3D;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Domena elementu materialowego 3D, ktorego grubosc wynika z profilu materialu.
/// </summary>
public sealed class MaterialSolid
{
    /// <summary>
    /// Tworzy element materialowy powiazany z profilem materialu.
    /// </summary>
    public MaterialSolid(
        Guid id,
        string name,
        MaterialProfile materialProfile,
        Mesh3D mesh,
        MaterialSolidOrientation? orientation = null,
        IEnumerable<CutoutFeature>? cutouts = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Material solid id cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Material solid name cannot be empty.", nameof(name));
        }

        Id = id;
        Name = name;
        MaterialProfile = materialProfile ?? throw new ArgumentNullException(nameof(materialProfile));
        Mesh = mesh ?? throw new ArgumentNullException(nameof(mesh));
        ValidateMeshThickness(Mesh, MaterialProfile);
        Orientation = orientation ?? MaterialSolidOrientation.Default;
        Cutouts = cutouts?.ToArray() ?? Array.Empty<CutoutFeature>();

        foreach (var cutout in Cutouts)
        {
            ValidateCutout(cutout, MaterialProfile.MinimumFingerWidth.Millimeters);
        }
    }

    /// <summary>
    /// Tworzy element materialowy z nowym identyfikatorem.
    /// </summary>
    public MaterialSolid(
        string name,
        MaterialProfile materialProfile,
        Mesh3D mesh,
        MaterialSolidOrientation? orientation = null,
        IEnumerable<CutoutFeature>? cutouts = null)
        : this(Guid.NewGuid(), name, materialProfile, mesh, orientation, cutouts)
    {
    }

    /// <summary>
    /// Tworzy prostopadloscian materialowy z prostokata 2D i grubosci profilu materialu.
    /// </summary>
    public static MaterialSolid FromRectangle(string name, RectangleEntity rectangle, MaterialProfile materialProfile)
    {
        if (rectangle is null)
        {
            throw new ArgumentNullException(nameof(rectangle));
        }

        if (materialProfile is null)
        {
            throw new ArgumentNullException(nameof(materialProfile));
        }

        var builder = new Contour3DBuilder();
        var part = builder.FromRectangle(rectangle, materialProfile.Thickness.Millimeters, name);

        return new MaterialSolid(name, materialProfile, part.Mesh);
    }

    /// <summary>
    /// Tworzy prostopadloscian materialowy z prostokata 2D i profilu materialu aktualnego dokumentu.
    /// </summary>
    public static MaterialSolid FromRectangle(string name, RectangleEntity rectangle, CadDocument document)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        if (document.MaterialProfile is null)
        {
            throw new InvalidOperationException("Document must have a material profile to create a material solid.");
        }

        return FromRectangle(name, rectangle, document.MaterialProfile);
    }

    /// <summary>
    /// Stabilny identyfikator elementu materialowego.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Nazwa elementu materialowego widoczna w modelu.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Profil materialu, z ktorego pochodzi grubosc elementu.
    /// </summary>
    public MaterialProfile MaterialProfile { get; }

    /// <summary>
    /// Mesh prostopadloscianu albo innej bryly materialowej.
    /// </summary>
    public Mesh3D Mesh { get; }

    /// <summary>
    /// Orientacja elementu materialowego w przestrzeni 3D.
    /// </summary>
    public MaterialSolidOrientation Orientation { get; }

    /// <summary>
    /// Wyciecia negatywowe osadzone w elemencie materialowym.
    /// </summary>
    public IReadOnlyList<CutoutFeature> Cutouts { get; }

    /// <summary>
    /// Grubosc elementu materialowego wynikajaca bezposrednio z profilu materialu.
    /// </summary>
    public Length Thickness => MaterialProfile.Thickness;

    /// <summary>
    /// Zwraca ten sam element materialowy z nowa orientacja w przestrzeni.
    /// </summary>
    public MaterialSolid WithOrientation(MaterialSolidOrientation orientation)
    {
        return new MaterialSolid(Id, Name, MaterialProfile, Mesh, orientation, Cutouts);
    }

    /// <summary>
    /// Zwraca ten sam element materialowy z dodanym wycieciem negatywowym.
    /// </summary>
    public MaterialSolid AddCutout(CutoutFeature cutout, double? minimumBridgeMillimeters = null)
    {
        if (cutout is null)
        {
            throw new ArgumentNullException(nameof(cutout));
        }

        ValidateCutout(cutout, minimumBridgeMillimeters ?? MaterialProfile.MinimumFingerWidth.Millimeters);
        return new MaterialSolid(Id, Name, MaterialProfile, Mesh, Orientation, Cutouts.Append(cutout));
    }

    /// <summary>
    /// Tworzy plaski podglad 2D plyty z wycieciami jako konturami wewnetrznymi.
    /// </summary>
    public FlatMaterialPreview CreateFlatPreview()
    {
        var bounds = Mesh.Bounds2D;
        var outer = new Polygon2D(new[]
        {
            new Point2D(bounds.MinX, bounds.MinY),
            new Point2D(bounds.MaxX, bounds.MinY),
            new Point2D(bounds.MaxX, bounds.MaxY),
            new Point2D(bounds.MinX, bounds.MaxY),
        });

        return new FlatMaterialPreview(outer, Cutouts.Select(cutout => cutout.Contour));
    }

    private static void ValidateMeshThickness(Mesh3D mesh, MaterialProfile materialProfile)
    {
        var minZ = mesh.Vertices.Min(vertex => vertex.Z);
        var maxZ = mesh.Vertices.Max(vertex => vertex.Z);
        var meshThickness = maxZ - minZ;
        var materialThickness = materialProfile.Thickness.Millimeters;

        if (Math.Abs(meshThickness - materialThickness) > GeometryTolerance.Default)
        {
            throw new ArgumentException(
                "Mesh thickness must match material profile thickness.",
                nameof(mesh));
        }
    }

    private void ValidateCutout(CutoutFeature cutout, double minimumBridgeMillimeters)
    {
        if (minimumBridgeMillimeters < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumBridgeMillimeters), "Minimum bridge cannot be negative.");
        }

        var bounds = Mesh.Bounds2D;
        foreach (var vertex in cutout.Contour.Vertices)
        {
            if (!bounds.Contains(vertex, GeometryTolerance.Default))
            {
                throw new ArgumentException("Cutout must fit inside material solid footprint.", nameof(cutout));
            }
        }

        var cutoutBounds = cutout.Bounds;
        var bridge = Math.Min(
            Math.Min(cutoutBounds.MinX - bounds.MinX, bounds.MaxX - cutoutBounds.MaxX),
            Math.Min(cutoutBounds.MinY - bounds.MinY, bounds.MaxY - cutoutBounds.MaxY));

        if (bridge < minimumBridgeMillimeters - GeometryTolerance.Default)
        {
            throw new ArgumentException("Cutout leaves a bridge smaller than the required minimum.", nameof(cutout));
        }
    }
}
