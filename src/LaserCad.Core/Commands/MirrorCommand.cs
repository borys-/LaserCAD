using LaserCad.Core.Documents;

namespace LaserCad.Core.Commands;

/// <summary>
/// Odbija encje szkicu wzgledem osi ukladu wspolrzednych.
/// </summary>
public sealed class MirrorCommand : ICommand
{
    public MirrorCommand(Guid sketchId, Guid entityId, SketchMirrorAxis axis)
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
        Axis = axis;
    }

    public Guid SketchId { get; }

    public Guid EntityId { get; }

    public SketchMirrorAxis Axis { get; }

    public CadDocument Execute(CadDocument document)
    {
        return Mirror(document);
    }

    public CadDocument Undo(CadDocument document)
    {
        return Mirror(document);
    }

    private CadDocument Mirror(CadDocument document)
    {
        return DocumentCommandHelpers.ReplaceSketch(
            document,
            SketchId,
            sketch => sketch.MirrorEntity(EntityId, Axis));
    }
}
