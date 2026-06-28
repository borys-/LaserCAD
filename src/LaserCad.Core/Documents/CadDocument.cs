using LaserCad.Core.Parameters;

namespace LaserCad.Core.Documents;

public sealed class CadDocument
{
    public CadDocument(
        Guid? id = null,
        string name = "Untitled",
        int formatVersion = 1,
        ParameterSet? parameters = null,
        IEnumerable<Layer>? layers = null,
        IEnumerable<Sketch>? sketches = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Document name cannot be empty.", nameof(name));
        }

        if (formatVersion <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(formatVersion), "Document format version must be positive.");
        }

        Id = id ?? Guid.NewGuid();
        Name = name;
        FormatVersion = formatVersion;
        Parameters = parameters ?? new ParameterSet();
        Layers = layers?.ToArray() ?? Array.Empty<Layer>();
        Sketches = sketches?.ToArray() ?? Array.Empty<Sketch>();

        if (Layers.Any(layer => layer is null))
        {
            throw new ArgumentException("Document layers cannot contain null values.", nameof(layers));
        }

        if (Sketches.Any(sketch => sketch is null))
        {
            throw new ArgumentException("Document sketches cannot contain null values.", nameof(sketches));
        }

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Document id cannot be empty.", nameof(id));
        }
    }

    public Guid Id { get; }

    public string Name { get; }

    public int FormatVersion { get; }

    public ParameterSet Parameters { get; }

    public IReadOnlyList<Layer> Layers { get; }

    public IReadOnlyList<Sketch> Sketches { get; }

    public CadDocument AddParameter(Parameter parameter)
    {
        return new CadDocument(Id, Name, FormatVersion, Parameters.Add(parameter), Layers, Sketches);
    }

    public CadDocument AddLayer(Layer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        return new CadDocument(Id, Name, FormatVersion, Parameters, Layers.Append(layer), Sketches);
    }

    public CadDocument AddSketch(Sketch sketch)
    {
        ArgumentNullException.ThrowIfNull(sketch);

        return new CadDocument(Id, Name, FormatVersion, Parameters, Layers, Sketches.Append(sketch));
    }
}
