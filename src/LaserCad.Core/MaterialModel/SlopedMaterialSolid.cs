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
        MaterialSolidOrientation? orientation = null,
        IEnumerable<CutoutFeature>? cutouts = null)
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
        Cutouts = cutouts?.ToArray() ?? Array.Empty<CutoutFeature>();

        foreach (var cutout in Cutouts)
        {
            ValidateCutoutOnFace(cutout, MaterialProfile.MinimumFingerWidth.Millimeters);
        }
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
    /// Wyciecia osadzone na nazwanych scianach bryly.
    /// </summary>
    public IReadOnlyList<CutoutFeature> Cutouts { get; }

    /// <summary>
    /// Zwraca bryle z dodanym wycieciem na wskazanej scianie.
    /// </summary>
    public SlopedMaterialSolid AddCutout(CutoutFeature cutout, double? minimumBridgeMillimeters = null)
    {
        if (cutout is null)
        {
            throw new ArgumentNullException(nameof(cutout));
        }

        if (cutout.FaceName is null)
        {
            throw new ArgumentException("Sloped material solid cutout must have a face name.", nameof(cutout));
        }

        ValidateCutoutOnFace(cutout, minimumBridgeMillimeters ?? MaterialProfile.MinimumFingerWidth.Millimeters);

        return new SlopedMaterialSolid(
            Id,
            Name,
            MaterialProfile,
            Options,
            Orientation,
            Cutouts.Append(cutout));
    }

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

        var parts = new[]
        {
            CreateRectanglePart("Front", width, frontHeight),
            CreateRectanglePart("Back", width, backHeight),
            CreateSidePart("Left side", depth, frontHeight, backHeight),
            CreateSidePart("Right side", depth, frontHeight, backHeight),
            CreateRectanglePart("Bottom", width, depth),
            CreateRectanglePart("Sloped top", width, slopedDepth)
        };

        return parts
            .Select(part => new UnfoldedMaterialPart(
                part.Name,
                part.OuterContour,
                Cutouts
                    .Where(cutout => string.Equals(cutout.FaceName, part.Name, StringComparison.OrdinalIgnoreCase))
                    .Select(cutout => cutout.Contour)))
            .ToArray();
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

    private void ValidateCutoutOnFace(CutoutFeature cutout, double minimumBridgeMillimeters)
    {
        if (minimumBridgeMillimeters < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumBridgeMillimeters), "Minimum bridge cannot be negative.");
        }

        var face = Unfold()
            .FirstOrDefault(part => string.Equals(part.Name, cutout.FaceName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException("Cutout face does not exist in sloped material solid.", nameof(cutout));

        foreach (var vertex in cutout.Contour.Vertices)
        {
            if (!IsPointInsideConvexPolygon(vertex, face.OuterContour))
            {
                throw new ArgumentException("Cutout must fit inside the unfolded face contour.", nameof(cutout));
            }
        }

        var faceBounds = face.OuterContour.Bounds;
        var cutoutBounds = cutout.Bounds;
        var bridge = Math.Min(
            Math.Min(cutoutBounds.MinX - faceBounds.MinX, faceBounds.MaxX - cutoutBounds.MaxX),
            Math.Min(cutoutBounds.MinY - faceBounds.MinY, faceBounds.MaxY - cutoutBounds.MaxY));

        if (bridge < minimumBridgeMillimeters - GeometryTolerance.Default)
        {
            throw new ArgumentException("Cutout leaves a bridge smaller than the required minimum.", nameof(cutout));
        }
    }

    private static bool IsPointInsideConvexPolygon(Point2D point, Polygon2D polygon)
    {
        var vertices = polygon.Vertices;
        var hasPositive = false;
        var hasNegative = false;

        for (var index = 0; index < vertices.Count; index++)
        {
            var a = vertices[index];
            var b = vertices[(index + 1) % vertices.Count];
            var cross = ((b.X - a.X) * (point.Y - a.Y)) - ((b.Y - a.Y) * (point.X - a.X));

            if (cross > GeometryTolerance.Default)
            {
                hasPositive = true;
            }
            else if (cross < -GeometryTolerance.Default)
            {
                hasNegative = true;
            }

            if (hasPositive && hasNegative)
            {
                return false;
            }
        }

        return true;
    }
}
