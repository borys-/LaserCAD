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
        MaterialSolidOrientation? orientation = null)
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
    }

    /// <summary>
    /// Tworzy element materialowy z nowym identyfikatorem.
    /// </summary>
    public MaterialSolid(
        string name,
        MaterialProfile materialProfile,
        Mesh3D mesh,
        MaterialSolidOrientation? orientation = null)
        : this(Guid.NewGuid(), name, materialProfile, mesh, orientation)
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
    /// Grubosc elementu materialowego wynikajaca bezposrednio z profilu materialu.
    /// </summary>
    public Length Thickness => MaterialProfile.Thickness;

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
}
