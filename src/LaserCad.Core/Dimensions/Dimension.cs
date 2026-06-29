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

        return Apply(sketch, Value);
    }

    /// <summary>
    /// Zwraca szkic z encja przebudowana wartoscia wymiaru albo powiazanego parametru.
    /// </summary>
    public Sketch Apply(Sketch sketch, ParameterSet parameters)
    {
        if (sketch is null)
        {
            throw new ArgumentNullException(nameof(sketch));
        }

        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        return Apply(sketch, ResolveValue(parameters));
    }

    private Sketch Apply(Sketch sketch, Length value)
    {
        var entity = sketch.Entities.FirstOrDefault(candidate => candidate.Id == EntityId)
            ?? throw new InvalidOperationException($"Sketch entity '{EntityId}' was not found.");

        var updatedEntity = ApplyToEntity(entity, value);

        return new Sketch(
            sketch.Id,
            sketch.Name,
            sketch.Entities.Select(candidate => candidate.Id == EntityId ? updatedEntity : candidate));
    }

    private Entity ApplyToEntity(Entity entity, Length value)
    {
        if (Kind == DimensionKind.Length && entity is LineEntity line)
        {
            return ApplyLength(line, value);
        }

        if (Kind == DimensionKind.Width && entity is RectangleEntity rectangle)
        {
            return ApplyRectangleWidth(rectangle, value);
        }

        if (Kind == DimensionKind.Height && entity is RectangleEntity heightRectangle)
        {
            return ApplyRectangleHeight(heightRectangle, value);
        }

        if (Kind == DimensionKind.Diameter && entity is CircleEntity circle)
        {
            return ApplyCircleDiameter(circle, value);
        }

        if (Kind == DimensionKind.Radius && entity is CircleEntity radiusCircle)
        {
            return ApplyCircleRadius(radiusCircle, value);
        }

        throw new InvalidOperationException($"Dimension kind '{Kind}' is not supported for entity '{entity.GetType().Name}'.");
    }

    private Length ResolveValue(ParameterSet parameters)
    {
        if (ParameterId is null)
        {
            return Value;
        }

        var parameterId = ParameterId.Value;
        var parameter = parameters.FindById(parameterId)
            ?? throw new ArgumentException($"Parameter '{parameterId}' was not found.", nameof(parameters));

        if (parameter.Type != ParameterType.Length || parameter.Value is not Length length)
        {
            throw new InvalidOperationException($"Parameter '{parameterId}' must be a Length parameter.");
        }

        if (length.Millimeters <= 0.0)
        {
            throw new InvalidOperationException($"Parameter '{parameterId}' must be positive.");
        }

        return length;
    }

    private static LineEntity ApplyLength(LineEntity line, Length value)
    {
        var direction = line.Segment.Direction;
        var end = new Point2D(
            line.Segment.Start.X + (direction.X * value.Millimeters),
            line.Segment.Start.Y + (direction.Y * value.Millimeters));

        return new LineEntity(
            new LineSegment2D(line.Segment.Start, end),
            line.Id,
            line.LayerName);
    }

    private static RectangleEntity ApplyRectangleWidth(RectangleEntity rectangle, Length value)
    {
        var bounds = rectangle.Bounds;
        var height = bounds.MaxY - bounds.MinY;

        return new RectangleEntity(
            new Point2D(bounds.MinX, bounds.MinY),
            value.Millimeters,
            height,
            rectangle.Id,
            rectangle.LayerName,
            rectangle.DimensionBindings);
    }

    private static RectangleEntity ApplyRectangleHeight(RectangleEntity rectangle, Length value)
    {
        var bounds = rectangle.Bounds;
        var width = bounds.MaxX - bounds.MinX;

        return new RectangleEntity(
            new Point2D(bounds.MinX, bounds.MinY),
            width,
            value.Millimeters,
            rectangle.Id,
            rectangle.LayerName,
            rectangle.DimensionBindings);
    }

    private static CircleEntity ApplyCircleDiameter(CircleEntity circle, Length value)
    {
        return new CircleEntity(
            new Circle2D(circle.Circle.Center, value.Millimeters / 2.0),
            circle.Id,
            circle.LayerName,
            circle.DimensionBindings);
    }

    private static CircleEntity ApplyCircleRadius(CircleEntity circle, Length value)
    {
        return new CircleEntity(
            new Circle2D(circle.Circle.Center, value.Millimeters),
            circle.Id,
            circle.LayerName,
            circle.DimensionBindings);
    }
}
