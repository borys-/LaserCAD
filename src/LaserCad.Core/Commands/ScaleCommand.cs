using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Commands;

/// <summary>
/// Skaluje encje szkicu wzgledem poczatku ukladu wspolrzednych.
/// </summary>
public sealed class ScaleCommand : ICommand
{
    public ScaleCommand(Guid sketchId, Guid entityId, double scaleX, double scaleY)
    {
        if (sketchId == Guid.Empty)
        {
            throw new ArgumentException("Sketch id cannot be empty.", nameof(sketchId));
        }

        if (entityId == Guid.Empty)
        {
            throw new ArgumentException("Entity id cannot be empty.", nameof(entityId));
        }

        if (Math.Abs(scaleX) <= GeometryTolerance.Default)
        {
            throw new ArgumentOutOfRangeException(nameof(scaleX), "Scale X cannot be zero.");
        }

        if (Math.Abs(scaleY) <= GeometryTolerance.Default)
        {
            throw new ArgumentOutOfRangeException(nameof(scaleY), "Scale Y cannot be zero.");
        }

        SketchId = sketchId;
        EntityId = entityId;
        ScaleX = scaleX;
        ScaleY = scaleY;
    }

    public Guid SketchId { get; }

    public Guid EntityId { get; }

    public double ScaleX { get; }

    public double ScaleY { get; }

    public CadDocument Execute(CadDocument document)
    {
        return DocumentCommandHelpers.ReplaceSketch(
            document,
            SketchId,
            sketch => sketch.ScaleEntity(EntityId, ScaleX, ScaleY));
    }

    public CadDocument Undo(CadDocument document)
    {
        return DocumentCommandHelpers.ReplaceSketch(
            document,
            SketchId,
            sketch => sketch.ScaleEntity(EntityId, 1.0 / ScaleX, 1.0 / ScaleY));
    }
}
