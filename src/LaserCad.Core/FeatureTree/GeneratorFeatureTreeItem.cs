using LaserCad.Core.Documents;

namespace LaserCad.Core.FeatureTree;

/// <summary>
/// Wpis drzewa historii reprezentujacy generator parametryczny i jego wynikowe szkice.
/// </summary>
public sealed class GeneratorFeatureTreeItem : FeatureTreeItem
{
    public GeneratorFeatureTreeItem(
        GeneratorInstance generator,
        IEnumerable<Sketch>? generatedSketches = null,
        Guid? id = null,
        string? name = null,
        bool isEnabled = true)
        : base(id, name ?? generator?.GeneratorType ?? "Generator", FeatureTreeItemKind.Generator, isEnabled)
    {
        if (generator is null)
        {
            throw new ArgumentNullException(nameof(generator));
        }

        Generator = generator;
        GeneratedSketches = generatedSketches?.ToArray() ?? Array.Empty<Sketch>();

        if (GeneratedSketches.Any(sketch => sketch is null))
        {
            throw new ArgumentException("Generated sketches cannot contain null values.", nameof(generatedSketches));
        }
    }

    /// <summary>
    /// Instancja generatora zapisana w dokumencie.
    /// </summary>
    public GeneratorInstance Generator { get; }

    /// <summary>
    /// Szkice utworzone przez generator w aktualnej przebudowie.
    /// </summary>
    public IReadOnlyList<Sketch> GeneratedSketches { get; }

    public override FeatureTreeItem WithEnabled(bool isEnabled)
    {
        return new GeneratorFeatureTreeItem(Generator, GeneratedSketches, Id, Name, isEnabled);
    }

    public override CadDocument Apply(CadDocument document)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        var rebuilt = document.AddGenerator(Generator);

        foreach (var sketch in GeneratedSketches)
        {
            rebuilt = rebuilt.AddSketch(sketch);
        }

        return rebuilt;
    }
}
