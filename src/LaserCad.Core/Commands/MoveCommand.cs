using LaserCad.Core.Documents;

namespace LaserCad.Core.Commands;

/// <summary>
/// Przesuwa encje szkicu o podany wektor w milimetrach.
/// </summary>
public sealed class MoveCommand : ICommand
{
    public MoveCommand(Guid sketchId, Guid entityId, double offsetX, double offsetY)
    {
        if (sketchId == Guid.Empty)
        {
            throw new ArgumentException("Sketch id cannot be empty.", nameof(sketchId));
        }

        if (entityId == Guid.Empty)
        {
            throw new ArgumentException("Entity id cannot be empty.", nameof(entityId));
        }

        SketchId = sketchId;
        EntityId = entityId;
        OffsetX = offsetX;
        OffsetY = offsetY;
    }

    public Guid SketchId { get; }

    public Guid EntityId { get; }

    public double OffsetX { get; }

    public double OffsetY { get; }

    public CadDocument Execute(CadDocument document)
    {
        return DocumentCommandHelpers.ReplaceSketch(
            document,
            SketchId,
            sketch => sketch.MoveEntity(EntityId, OffsetX, OffsetY));
    }

    public CadDocument Undo(CadDocument document)
    {
        return DocumentCommandHelpers.ReplaceSketch(
            document,
            SketchId,
            sketch => sketch.MoveEntity(EntityId, -OffsetX, -OffsetY));
    }
}
