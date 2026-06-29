using LaserCad.Core.Documents;

namespace LaserCad.Core.Commands;

/// <summary>
/// Usuwa encje ze szkicu i pozwala przywrocic ja przez undo.
/// </summary>
public sealed class DeleteCommand : ICommand
{
    public DeleteCommand(Guid sketchId, Entity entity)
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

    public Guid EntityId => Entity.Id;

    public CadDocument Execute(CadDocument document)
    {
        return DocumentCommandHelpers.ReplaceSketch(
            document,
            SketchId,
            sketch => sketch.RemoveEntity(EntityId));
    }

    public CadDocument Undo(CadDocument document)
    {
        return DocumentCommandHelpers.ReplaceSketch(
            document,
            SketchId,
            sketch => sketch.AddEntity(Entity));
    }
}
