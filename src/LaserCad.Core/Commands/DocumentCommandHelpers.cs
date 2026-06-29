using LaserCad.Core.Documents;

namespace LaserCad.Core.Commands;

internal static class DocumentCommandHelpers
{
    public static CadDocument ReplaceSketch(CadDocument document, Guid sketchId, Func<Sketch, Sketch> update)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        if (update is null)
        {
            throw new ArgumentNullException(nameof(update));
        }

        bool found = false;
        Sketch[] sketches = document.Sketches
            .Select(sketch =>
            {
                if (sketch.Id != sketchId)
                {
                    return sketch;
                }

                found = true;
                return update(sketch);
            })
            .ToArray();

        if (!found)
        {
            throw new InvalidOperationException($"Sketch '{sketchId}' was not found.");
        }

        return new CadDocument(
            document.Id,
            document.Name,
            document.FormatVersion,
            document.Parameters,
            document.Layers,
            sketches,
            document.Generators,
            document.MaterialProfile);
    }
}
