using LaserCad.Geometry;

namespace LaserCad.Core.Documents;

/// <summary>
/// Encja polilinii w szkicu.
/// </summary>
public sealed class PolylineEntity : Entity
{
    /// <summary>
    /// Tworzy encje polilinii na podstawie geometrii 2D.
    /// </summary>
    public PolylineEntity(Polyline2D polyline, Guid? id = null, string layerName = "Cut")
        : base(id, layerName)
    {
        Polyline = polyline ?? throw new ArgumentNullException(nameof(polyline));
    }

    /// <summary>
    /// Geometria polilinii.
    /// </summary>
    public Polyline2D Polyline { get; }

    /// <inheritdoc />
    public override BoundingBox Bounds => Polyline.Bounds;

    /// <inheritdoc />
    public override ISketchEntity Transform(Matrix3x3 transform)
    {
        return new PolylineEntity(Polyline.Transform(transform), Id, LayerName);
    }
}
