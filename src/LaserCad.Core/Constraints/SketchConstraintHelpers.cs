using LaserCad.Core.Documents;
using LaserCad.Geometry;

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
        }).ToArray();

        if (!replaced)
        {
            throw new InvalidOperationException($"Sketch entity '{replacement.Id}' was not found.");
        }

        return new Sketch(sketch.Id, sketch.Name, entities);
    }

    public static Point2D GetPoint(Sketch sketch, SketchPointReference reference)
    {
        var line = GetLine(sketch, reference.EntityId);

        return reference.PointRole switch
        {
            SketchPointRole.Start => line.Segment.Start,
            SketchPointRole.End => line.Segment.End,
            _ => throw new ArgumentOutOfRangeException(nameof(reference), reference.PointRole, "Unsupported sketch point role."),
        };
    }

    public static Sketch MovePoint(Sketch sketch, SketchPointReference reference, Point2D point)
    {
        var line = GetLine(sketch, reference.EntityId);
        var segment = reference.PointRole switch
        {
            SketchPointRole.Start => new LineSegment2D(point, line.Segment.End),
            SketchPointRole.End => new LineSegment2D(line.Segment.Start, point),
            _ => throw new ArgumentOutOfRangeException(nameof(reference), reference.PointRole, "Unsupported sketch point role."),
        };
        var replacement = new LineEntity(segment, line.Id, line.LayerName);

        return ReplaceEntity(sketch, replacement);
    }
}
