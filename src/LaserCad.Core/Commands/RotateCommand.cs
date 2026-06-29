using LaserCad.Core.Documents;

namespace LaserCad.Core.Commands;

/// <summary>
/// Obraca encje szkicu wokol poczatku ukladu wspolrzednych.
/// </summary>
public sealed class RotateCommand : ICommand
{
    public RotateCommand(Guid sketchId, Guid entityId, double angleRadians)
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
        AngleRadians = angleRadians;
    }

    public Guid SketchId { get; }

    public Guid EntityId { get; }

    public double AngleRadians { get; }

    public CadDocument Execute(CadDocument document)
    {
        return DocumentCommandHelpers.ReplaceSketch(
            document,
            SketchId,
            sketch => sketch.RotateEntity(EntityId, AngleRadians));
    }

    public CadDocument Undo(CadDocument document)
    {
        return DocumentCommandHelpers.ReplaceSketch(
            document,
            SketchId,
            sketch => sketch.RotateEntity(EntityId, -AngleRadians));
    }
}
