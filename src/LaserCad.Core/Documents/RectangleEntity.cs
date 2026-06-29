using LaserCad.Geometry;
using LaserCad.Geometry.Units;
using LaserCad.Core.Parameters;

namespace LaserCad.Core.Documents;

/// <summary>
/// Encja prostokata opisana czterema punktami w kolejnosci obwodu.
/// Uzywaj jej dla prostokatow osiowych i po transformacjach afinicznych.
/// </summary>
public sealed class RectangleEntity : Entity
{
    private readonly Point2D[] corners;
    private readonly IReadOnlyList<Point2D> readOnlyCorners;

    /// <summary>
    /// Tworzy prostokat osiowy z lewego dolnego rogu, szerokosci i wysokosci.
    /// </summary>
    public RectangleEntity(
        Point2D origin,
        double width,
        double height,
        Guid? id = null,
        string layerName = "Cut",
        IEnumerable<EntityDimensionBinding>? dimensionBindings = null)
        : this(CreateCorners(origin, width, height), id, layerName, dimensionBindings)
    {
    }

    /// <summary>
    /// Tworzy prostokat z czterech naroznikow.
    /// Kolejnosc punktow powinna odpowiadac przejsciu po obwodzie.
    /// </summary>
    public RectangleEntity(
        IEnumerable<Point2D> corners,
        Guid? id = null,
        string layerName = "Cut",
        IEnumerable<EntityDimensionBinding>? dimensionBindings = null)
        : base(id, layerName, dimensionBindings)
    {
        ArgumentNullException.ThrowIfNull(corners);

        this.corners = corners.ToArray();

        if (this.corners.Length != 4)
        {
            throw new ArgumentException("Rectangle must contain exactly four corners.", nameof(corners));
        }

        readOnlyCorners = Array.AsReadOnly(this.corners);
    }

    /// <summary>
    /// Narozniki prostokata w kolejnosci obwodu.
    /// </summary>
    public IReadOnlyList<Point2D> Corners => readOnlyCorners;

    /// <inheritdoc />
    public override BoundingBox Bounds => BoundingBox.FromPoints(corners);

    /// <inheritdoc />
    public override ISketchEntity Transform(Matrix3x3 transform)
    {
        return new RectangleEntity(corners.Select(transform.Transform), Id, LayerName, DimensionBindings);
    }

    /// <inheritdoc />
    public override Entity Copy(Guid? id = null)
    {
        return new RectangleEntity(corners, id, LayerName, DimensionBindings);
    }

    /// <summary>
    /// Zwraca prostokat z dopisanym powiazaniem wymiaru z parametrem.
    /// </summary>
    public RectangleEntity BindDimension(EntityDimensionBinding binding)
    {
        ArgumentNullException.ThrowIfNull(binding);

        return new RectangleEntity(corners, Id, LayerName, DimensionBindings.Append(binding));
    }

    /// <summary>
    /// Zwraca prostokat przebudowany na podstawie powiazanych parametrow.
    /// MVP obsluguje prostokaty osiowe opisane przez bounding box.
    /// </summary>
    public RectangleEntity RebuildFromParameters(ParameterSet parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var bounds = Bounds;
        var width = bounds.MaxX - bounds.MinX;
        var height = bounds.MaxY - bounds.MinY;

        foreach (var binding in DimensionBindings)
        {
            if (binding.Dimension == EntityDimensionKind.Width)
            {
                width = GetLengthParameter(parameters, binding).Millimeters;
            }

            if (binding.Dimension == EntityDimensionKind.Height)
            {
                height = GetLengthParameter(parameters, binding).Millimeters;
            }
        }

        return new RectangleEntity(
            new Point2D(bounds.MinX, bounds.MinY),
            width,
            height,
            Id,
            LayerName,
            DimensionBindings);
    }

    private static Point2D[] CreateCorners(Point2D origin, double width, double height)
    {
        if (width <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
        }

        if (height <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");
        }

        return new[]
        {
            origin,
            new Point2D(origin.X + width, origin.Y),
            new Point2D(origin.X + width, origin.Y + height),
            new Point2D(origin.X, origin.Y + height),
        };
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
