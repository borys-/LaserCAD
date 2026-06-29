using LaserCad.Geometry;
using LaserCad.Geometry.Units;
using LaserCad.Core.Parameters;

namespace LaserCad.Core.Documents;

/// <summary>
/// Encja okregu w szkicu.
/// </summary>
public sealed class CircleEntity : Entity
{
    /// <summary>
    /// Tworzy encje okregu na podstawie geometrii 2D.
    /// </summary>
    public CircleEntity(
        Circle2D circle,
        Guid? id = null,
        string layerName = "Cut",
        IEnumerable<EntityDimensionBinding>? dimensionBindings = null)
        : base(id, layerName, dimensionBindings)
    {
        Circle = circle;
    }

    /// <summary>
    /// Geometria okregu.
    /// </summary>
    public Circle2D Circle { get; }

    /// <inheritdoc />
    public override BoundingBox Bounds => Circle.Bounds;

    /// <inheritdoc />
    public override ISketchEntity Transform(Matrix3x3 transform)
    {
        return new CircleEntity(Circle.Transform(transform), Id, LayerName, DimensionBindings);
    }

    /// <inheritdoc />
    public override Entity Copy(Guid? id = null)
    {
        return new CircleEntity(Circle, id, LayerName, DimensionBindings);
    }

    /// <summary>
    /// Zwraca okrag z dopisanym powiazaniem wymiaru z parametrem.
    /// </summary>
    public CircleEntity BindDimension(EntityDimensionBinding binding)
    {
        if (binding is null)
        {
            throw new ArgumentNullException(nameof(binding));
        }

        return new CircleEntity(Circle, Id, LayerName, DimensionBindings.Append(binding));
    }

    /// <summary>
    /// Zwraca okrag przebudowany na podstawie powiazanych parametrow.
    /// MVP obsluguje srednice okregu.
    /// </summary>
    public CircleEntity RebuildFromParameters(ParameterSet parameters)
    {
        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        var radius = Circle.Radius;

        foreach (var binding in DimensionBindings)
        {
            if (binding.Dimension == EntityDimensionKind.Diameter)
            {
                radius = GetLengthParameter(parameters, binding).Millimeters / 2.0;
            }
        }

        return new CircleEntity(
            new Circle2D(Circle.Center, radius),
            Id,
            LayerName,
            DimensionBindings);
    }

    private static Length GetLengthParameter(ParameterSet parameters, EntityDimensionBinding binding)
    {
        var parameter = parameters.FindById(binding.ParameterId)
            ?? throw new ArgumentException($"Parameter '{binding.ParameterId}' was not found.", nameof(parameters));

        if (parameter.Type != ParameterType.Length || parameter.Value is not Length length)
        {
            throw new InvalidOperationException($"Parameter '{binding.ParameterId}' must be a Length parameter.");
        }

        return length;
    }
}
