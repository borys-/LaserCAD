using LaserCad.Core.Parameters;

namespace LaserCad.Core.Documents;

public sealed class CadDocument
{
    public CadDocument(
        Guid? id = null,
        string name = "Untitled",
        int formatVersion = 1,
        ParameterSet? parameters = null,
        IEnumerable<Layer>? layers = null)
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

        if (Layers.Any(layer => layer is null))
        {
            throw new ArgumentException("Document layers cannot contain null values.", nameof(layers));
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

    public CadDocument AddParameter(Parameter parameter)
    {
        return new CadDocument(Id, Name, FormatVersion, Parameters.Add(parameter), Layers);
    }

    public CadDocument AddLayer(Layer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        return new CadDocument(Id, Name, FormatVersion, Parameters, Layers.Append(layer));
    }
}
