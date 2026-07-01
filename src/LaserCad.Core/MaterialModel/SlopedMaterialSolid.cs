using LaserCad.Core.Documents;
using LaserCad.Core.Preview3D;
using LaserCad.Geometry;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Bryla materialowa z jedna pochyla gorna sciana, modelowana jako kontrolowany wariant plytowy.
/// </summary>
public sealed class SlopedMaterialSolid
{
    /// <summary>
    /// Tworzy bryle z pochyleniem wynikajacym z wysokosci przedniej i tylnej.
    /// </summary>
    public SlopedMaterialSolid(
        Guid id,
        string name,
        MaterialProfile materialProfile,
        SlopedMaterialSolidOptions options,
        MaterialSolidOrientation? orientation = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Sloped material solid id cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Sloped material solid name cannot be empty.", nameof(name));
        }

        Id = id;
        Name = name;
        MaterialProfile = materialProfile ?? throw new ArgumentNullException(nameof(materialProfile));
        Options = options ?? throw new ArgumentNullException(nameof(options));
        Orientation = orientation ?? MaterialSolidOrientation.Default;
        PreviewMesh = CreatePreviewMesh(options);
    }

    /// <summary>
    /// Tworzy bryle z nowym identyfikatorem.
    /// </summary>
    public SlopedMaterialSolid(
        string name,
        MaterialProfile materialProfile,
        SlopedMaterialSolidOptions options,
        MaterialSolidOrientation? orientation = null)
        : this(Guid.NewGuid(), name, materialProfile, options, orientation)
    {
    }

    /// <summary>
    /// Stabilny identyfikator bryly.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Nazwa bryly widoczna w modelu.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Profil materialu uzyty dla plyt tworzacych bryle.
    /// </summary>
    public MaterialProfile MaterialProfile { get; }

    /// <summary>
    /// Parametry wymiarowe bryly.
    /// </summary>
    public SlopedMaterialSolidOptions Options { get; }

    /// <summary>
    /// Orientacja bryly w przestrzeni 3D.
    /// </summary>
    public MaterialSolidOrientation Orientation { get; }

    /// <summary>
    /// Mesh pogladowy bryly z pochyla sciana.
    /// </summary>
    public Mesh3D PreviewMesh { get; }

    /// <summary>
    /// Rozwija bryle na plaskie czesci materialowe.
    /// </summary>
    public IReadOnlyList<UnfoldedMaterialPart> Unfold()
    {
        var width = Options.Width.Millimeters;
        var depth = Options.Depth.Millimeters;
        var frontHeight = Options.FrontHeight.Millimeters;
        var backHeight = Options.BackHeight.Millimeters;
        var slopedDepth = Options.SlopedDepth.Millimeters;

        return new[]
        {
            CreateRectanglePart("Front", width, frontHeight),
            CreateRectanglePart("Back", width, backHeight),
            CreateSidePart("Left side", depth, frontHeight, backHeight),
            CreateSidePart("Right side", depth, frontHeight, backHeight),
            CreateRectanglePart("Bottom", width, depth),
            CreateRectanglePart("Sloped top", width, slopedDepth)
        };
    }

    private static Mesh3D CreatePreviewMesh(SlopedMaterialSolidOptions options)
    {
        var width = options.Width.Millimeters;
        var depth = options.Depth.Millimeters;
        var frontHeight = options.FrontHeight.Millimeters;
        var backHeight = options.BackHeight.Millimeters;

        var vertices = new[]
        {
            new Point3D(0.0, 0.0, 0.0),
            new Point3D(width, 0.0, 0.0),
            new Point3D(width, depth, 0.0),
            new Point3D(0.0, depth, 0.0),
            new Point3D(0.0, 0.0, frontHeight),
            new Point3D(width, 0.0, frontHeight),
            new Point3D(width, depth, backHeight),
            new Point3D(0.0, depth, backHeight)
        };

        var triangles = new[]
        {
            0, 2, 1, 0, 3, 2,
            0, 1, 5, 0, 5, 4,
            3, 7, 6, 3, 6, 2,
            0, 4, 7, 0, 7, 3,
            1, 2, 6, 1, 6, 5,
            4, 5, 6, 4, 6, 7
        };

        return new Mesh3D(vertices, triangles);
    }

    private static UnfoldedMaterialPart CreateRectanglePart(string name, double width, double height)
    {
        return new UnfoldedMaterialPart(
            name,
            new Polygon2D(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(width, 0.0),
                new Point2D(width, height),
                new Point2D(0.0, height)
            }));
    }

    private static UnfoldedMaterialPart CreateSidePart(
        string name,
        double depth,
        double frontHeight,
        double backHeight)
    {
        return new UnfoldedMaterialPart(
            name,
            new Polygon2D(new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(depth, 0.0),
                new Point2D(depth, backHeight),
                new Point2D(0.0, frontHeight)
            }));
    }
}
