using LaserCad.Geometry;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Wyciecie negatywowe lezace na powierzchni elementu materialowego.
/// </summary>
public sealed class CutoutFeature
{
    private const int CircleSegmentCount = 48;

    /// <summary>
    /// Tworzy wyciecie z gotowego konturu.
    /// </summary>
    public CutoutFeature(Guid id, string name, CutoutFeatureKind kind, Polygon2D contour, string? faceName = null)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Cutout id cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Cutout name cannot be empty.", nameof(name));
        }

        Id = id;
        Name = name;
        Kind = kind;
        Contour = contour ?? throw new ArgumentNullException(nameof(contour));
        FaceName = string.IsNullOrWhiteSpace(faceName) ? null : faceName;
    }

    /// <summary>
    /// Tworzy prostokatne wyciecie.
    /// </summary>
    public static CutoutFeature Rectangle(string name, Point2D origin, double width, double height, string? faceName = null)
    {
        if (width <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Cutout width must be positive.");
        }

        if (height <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "Cutout height must be positive.");
        }

        return new CutoutFeature(
            Guid.NewGuid(),
            name,
            CutoutFeatureKind.Rectangle,
            new Polygon2D(new[]
            {
                origin,
                new Point2D(origin.X + width, origin.Y),
                new Point2D(origin.X + width, origin.Y + height),
                new Point2D(origin.X, origin.Y + height),
            }),
            faceName);
    }

    /// <summary>
    /// Tworzy okragle wyciecie jako wielokatowy kontur produkcyjny MVP.
    /// </summary>
    public static CutoutFeature Circle(string name, Point2D center, double radius, string? faceName = null)
    {
        if (radius <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(radius), "Cutout radius must be positive.");
        }

        var points = Enumerable.Range(0, CircleSegmentCount)
            .Select(index =>
            {
                var angle = Math.PI * 2.0 * index / CircleSegmentCount;
                return new Point2D(
                    center.X + (Math.Cos(angle) * radius),
                    center.Y + (Math.Sin(angle) * radius));
            })
            .ToArray();

        return new CutoutFeature(Guid.NewGuid(), name, CutoutFeatureKind.Circle, new Polygon2D(points), faceName);
    }

    /// <summary>
    /// Tworzy wyciecie z zamknietej polilinii.
    /// </summary>
    public static CutoutFeature FromClosedPolyline(string name, Polyline2D polyline, string? faceName = null)
    {
        if (polyline is null)
        {
            throw new ArgumentNullException(nameof(polyline));
        }

        if (!polyline.IsClosed)
        {
            throw new ArgumentException("Cutout polyline must be closed.", nameof(polyline));
        }

        return new CutoutFeature(
            Guid.NewGuid(),
            name,
            CutoutFeatureKind.Polyline,
            new Polygon2D(polyline.Points),
            faceName);
    }

    /// <summary>
    /// Stabilny identyfikator wyciecia.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Nazwa wyciecia.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Typ wyciecia.
    /// </summary>
    public CutoutFeatureKind Kind { get; }

    /// <summary>
    /// Kontur wewnetrzny wyciecia w lokalnym ukladzie sciany.
    /// </summary>
    public Polygon2D Contour { get; }

    /// <summary>
    /// Opcjonalna nazwa sciany, na ktorej osadzono wyciecie.
    /// </summary>
    public string? FaceName { get; }

    /// <summary>
    /// Zakres konturu wyciecia.
    /// </summary>
    public BoundingBox Bounds => Contour.Bounds;
}
