using LaserCad.Core.Documents;

namespace LaserCad.Core.Commands;

/// <summary>
/// Dodaje encje do szkicu i pozwala usunac ja przez undo.
/// </summary>
public sealed class AddEntityCommand : ICommand
{
    public AddEntityCommand(Guid sketchId, Entity entity)
    {
        if (sketchId == Guid.Empty)
        {
            throw new ArgumentException("Sketch id cannot be empty.", nameof(sketchId));
        }

        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        SketchId = sketchId;
        Entity = entity;
    }

    public Guid SketchId { get; }

    public Entity Entity { get; }

    public CadDocument Execute(CadDocument document)
    {
        return DocumentCommandHelpers.ReplaceSketch(
            document,
            SketchId,
            sketch => sketch.AddEntity(Entity));
    }

    public CadDocument Undo(CadDocument document)
    {
        return DocumentCommandHelpers.ReplaceSketch(
            document,
            SketchId,
            sketch => sketch.RemoveEntity(Entity.Id));
    }
}
