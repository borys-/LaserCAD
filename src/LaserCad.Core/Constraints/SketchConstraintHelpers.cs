using LaserCad.Core.Documents;

namespace LaserCad.Core.Constraints;

internal static class SketchConstraintHelpers
{
    public static LineEntity GetLine(Sketch sketch, Guid entityId)
    {
        return sketch.Entities.FirstOrDefault(entity => entity.Id == entityId) as LineEntity
            ?? throw new InvalidOperationException($"Line entity '{entityId}' was not found.");
    }

    public static Sketch ReplaceEntity(Sketch sketch, Entity replacement)
    {
        if (replacement is null)
        {
            throw new ArgumentNullException(nameof(replacement));
        }

        var replaced = false;
        var entities = sketch.Entities.Select(entity =>
        {
            if (entity.Id != replacement.Id)
            {
                return entity;
            }

            replaced = true;
            return replacement;
        });

        if (!replaced)
        {
            throw new InvalidOperationException($"Sketch entity '{replacement.Id}' was not found.");
        }

        return new Sketch(sketch.Id, sketch.Name, entities);
    }
}
