namespace LaserCad.Core.Documents;

public sealed class CadDocument
{
    public CadDocument(Guid? id = null, string name = "Untitled", int formatVersion = 1)
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

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Document id cannot be empty.", nameof(id));
        }
    }

    public Guid Id { get; }

    public string Name { get; }

    public int FormatVersion { get; }
}
