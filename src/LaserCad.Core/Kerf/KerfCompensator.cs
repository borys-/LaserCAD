using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Kerf;

/// <summary>
/// Kompensuje kerf dla prostych, zamknietych, wypuklych konturow szkicu.
/// </summary>
public static class KerfCompensator
{
    /// <summary>
    /// Zwraca szkic skompensowany wedlug podanych opcji.
    /// MVP obsluguje prostokaty i zamkniete polilinie wypukle.
    /// </summary>
    public static Sketch Compensate(Sketch sketch, KerfCompensationOptions options)
    {
        if (sketch is null)
        {
            throw new ArgumentNullException(nameof(sketch));
        }

        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var compensated = new Sketch(sketch.Id, $"{sketch.Name} - kerf");

        foreach (Entity entity in sketch.Entities)
        {
            compensated = compensated.AddEntity(CompensateEntity(entity, options));
        }

        return compensated;
    }

    /// <summary>
    /// Zwraca podglad szkicu przed i po kompensacji kerfu.
    /// </summary>
    public static KerfCompensationPreview CreatePreview(Sketch sketch, KerfCompensationOptions options)
    {
        if (sketch is null)
        {
            throw new ArgumentNullException(nameof(sketch));
        }

        return new KerfCompensationPreview(sketch, Compensate(sketch, options));
    }

    /// <summary>
    /// Klasyfikuje domkniety kontur na podstawie orientacji polygonu.
    /// W MVP obieg przeciwny do wskazowek zegara oznacza kontur zewnetrzny.
    /// </summary>
    public static KerfContourClassification Classify(Polygon2D polygon)
    {
        if (polygon is null)
        {
            throw new ArgumentNullException(nameof(polygon));
        }

        return polygon.Orientation == PolygonOrientation.Counterclockwise
            ? KerfContourClassification.Outer
            : KerfContourClassification.Inner;
    }

    private static Entity CompensateEntity(Entity entity, KerfCompensationOptions options)
    {
        Polygon2D? polygon = TryCreatePolygon(entity);

        if (polygon is null)
        {
            return entity;
        }

        Polygon2D compensatedPolygon = Offset(polygon, options);
        return new PolylineEntity(
            new Polyline2D(compensatedPolygon.Vertices, isClosed: true),
            entity.Id,
            entity.LayerName);
    }

    private static Polygon2D Offset(Polygon2D polygon, KerfCompensationOptions options)
    {
        KerfContourClassification classification = Classify(polygon);
        bool positiveOuter = options.Mode == KerfCompensationMode.Positive &&
            classification == KerfContourClassification.Outer;
        bool negativeInner = options.Mode == KerfCompensationMode.Negative &&
            classification == KerfContourClassification.Inner;

        if (positiveOuter || negativeInner)
        {
            return PolygonOffset2D.OffsetOuter(polygon, options.OffsetDistanceMillimeters);
        }

        return PolygonOffset2D.OffsetInner(polygon, options.OffsetDistanceMillimeters);
    }

    private static Polygon2D? TryCreatePolygon(Entity entity)
    {
        if (entity is RectangleEntity rectangle)
        {
            return new Polygon2D(rectangle.Corners);
        }

        if (entity is PolylineEntity polyline && polyline.Polyline.IsClosed)
        {
            return new Polygon2D(polyline.Polyline.Points);
        }

        return null;
    }
}
