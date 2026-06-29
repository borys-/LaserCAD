using LaserCad.Geometry;

namespace LaserCad.Core.Documents;

/// <summary>
/// Encja okregu w szkicu.
/// </summary>
public sealed class CircleEntity : Entity
{
    /// <summary>
    /// Tworzy encje okregu na podstawie geometrii 2D.
    /// </summary>
    public CircleEntity(Circle2D circle, Guid? id = null, string layerName = "Cut")
        : base(id, layerName)
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
        return new CircleEntity(Circle.Transform(transform), Id, LayerName);
    }
}
