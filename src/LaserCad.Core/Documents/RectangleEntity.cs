using LaserCad.Geometry;

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
    public RectangleEntity(Point2D origin, double width, double height, Guid? id = null, string layerName = "Cut")
        : this(CreateCorners(origin, width, height), id, layerName)
    {
    }

    /// <summary>
    /// Tworzy prostokat z czterech naroznikow.
    /// Kolejnosc punktow powinna odpowiadac przejsciu po obwodzie.
    /// </summary>
    public RectangleEntity(IEnumerable<Point2D> corners, Guid? id = null, string layerName = "Cut")
        : base(id, layerName)
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
        return new RectangleEntity(corners.Select(transform.Transform), Id, LayerName);
    }

    /// <inheritdoc />
    public override Entity Copy(Guid? id = null)
    {
        return new RectangleEntity(corners, id, LayerName);
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
}
