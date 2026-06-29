using LaserCad.Core.Documents;
using LaserCad.Core.Parameters;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.Dimensions;

/// <summary>
/// Wymiar szkicu opisujacy intencje rozmiaru wybranej encji.
/// </summary>
public sealed class Dimension
{
    /// <summary>
    /// Tworzy wymiar szkicu dla wskazanej encji.
    /// </summary>
    public Dimension(
        Guid entityId,
        DimensionKind kind,
        Length value,
        Guid? id = null,
        string name = "Dimension",
        ParameterId? parameterId = null)
    {
        if (entityId == Guid.Empty)
        {
            throw new ArgumentException("Dimension entity id cannot be empty.", nameof(entityId));
        }

        if (id == Guid.Empty)
        {
            throw new ArgumentException("Dimension id cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Dimension name cannot be empty.", nameof(name));
        }

        if (value.Millimeters <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Dimension value must be positive.");
        }

        Id = id ?? Guid.NewGuid();
        EntityId = entityId;
        Kind = kind;
        Value = value;
        Name = name;
        ParameterId = parameterId;
    }

    /// <summary>
    /// Stabilny identyfikator wymiaru.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identyfikator encji, ktorej dotyczy wymiar.
    /// </summary>
    public Guid EntityId { get; }

    /// <summary>
    /// Rodzaj wymiaru.
    /// </summary>
    public DimensionKind Kind { get; }

    /// <summary>
    /// Nazwa wymiaru wyswietlana w UI lub diagnostyce.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Wartosc wymiaru w milimetrach domenowych.
    /// </summary>
    public Length Value { get; }

    /// <summary>
    /// Opcjonalny parametr dokumentu sterujacy wymiarem.
    /// </summary>
    public ParameterId? ParameterId { get; }

    /// <summary>
    /// Zwraca wymiar powiazany z parametrem dokumentu.
    /// </summary>
    public Dimension BindToParameter(ParameterId parameterId)
    {
        return new Dimension(EntityId, Kind, Value, Id, Name, parameterId);
    }

    /// <summary>
    /// Zwraca szkic z encja przebudowana tak, aby spelniala wartosc wymiaru.
    /// </summary>
    public Sketch Apply(Sketch sketch)
    {
        if (sketch is null)
        {
            throw new ArgumentNullException(nameof(sketch));
        }

        var entity = sketch.Entities.FirstOrDefault(candidate => candidate.Id == EntityId)
            ?? throw new InvalidOperationException($"Sketch entity '{EntityId}' was not found.");

        var updatedEntity = ApplyToEntity(entity);

        return new Sketch(
            sketch.Id,
            sketch.Name,
            sketch.Entities.Select(candidate => candidate.Id == EntityId ? updatedEntity : candidate));
    }

    private Entity ApplyToEntity(Entity entity)
    {
        if (Kind == DimensionKind.Length && entity is LineEntity line)
        {
            return ApplyLength(line);
        }

        if (Kind == DimensionKind.Width && entity is RectangleEntity rectangle)
        {
            return ApplyRectangleWidth(rectangle);
        }

        if (Kind == DimensionKind.Height && entity is RectangleEntity heightRectangle)
        {
            return ApplyRectangleHeight(heightRectangle);
        }

        if (Kind == DimensionKind.Diameter && entity is CircleEntity circle)
        {
            return ApplyCircleDiameter(circle);
        }

        throw new InvalidOperationException($"Dimension kind '{Kind}' is not supported for entity '{entity.GetType().Name}'.");
    }

    private LineEntity ApplyLength(LineEntity line)
    {
        var direction = line.Segment.Direction;
        var end = new Point2D(
            line.Segment.Start.X + (direction.X * Value.Millimeters),
            line.Segment.Start.Y + (direction.Y * Value.Millimeters));

        return new LineEntity(
            new LineSegment2D(line.Segment.Start, end),
            line.Id,
            line.LayerName);
    }

    private RectangleEntity ApplyRectangleWidth(RectangleEntity rectangle)
    {
        var bounds = rectangle.Bounds;
        var height = bounds.MaxY - bounds.MinY;

        return new RectangleEntity(
            new Point2D(bounds.MinX, bounds.MinY),
            Value.Millimeters,
            height,
            rectangle.Id,
            rectangle.LayerName,
            rectangle.DimensionBindings);
    }

    private RectangleEntity ApplyRectangleHeight(RectangleEntity rectangle)
    {
        var bounds = rectangle.Bounds;
        var width = bounds.MaxX - bounds.MinX;

        return new RectangleEntity(
            new Point2D(bounds.MinX, bounds.MinY),
            width,
            Value.Millimeters,
            rectangle.Id,
            rectangle.LayerName,
            rectangle.DimensionBindings);
    }

    private CircleEntity ApplyCircleDiameter(CircleEntity circle)
    {
        return new CircleEntity(
            new Circle2D(circle.Circle.Center, Value.Millimeters / 2.0),
            circle.Id,
            circle.LayerName,
            circle.DimensionBindings);
    }
}
