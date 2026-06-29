using LaserCad.Geometry;

namespace LaserCad.Core.Documents;

/// <summary>
/// Encja odcinka w szkicu.
/// Przechowuje geometrie liniowa oraz wspolne metadane encji dokumentu.
/// </summary>
public sealed class LineEntity : Entity
{
    /// <summary>
    /// Tworzy encje linii na podstawie odcinka 2D.
    /// </summary>
    public LineEntity(LineSegment2D segment, Guid? id = null, string layerName = "Cut")
        : base(id, layerName)
    {
        Segment = segment;
    }

    /// <summary>
    /// Odcinek geometryczny encji.
    /// </summary>
    public LineSegment2D Segment { get; }

    /// <inheritdoc />
    public override BoundingBox Bounds => Segment.Bounds;

    /// <inheritdoc />
    public override ISketchEntity Transform(Matrix3x3 transform)
    {
        return new LineEntity(Segment.Transform(transform), Id, LayerName);
    }
}
