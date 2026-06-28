namespace LaserCad.Core.Documents;

public sealed class CadDocument
{
    public CadDocument(Guid? id = null, string name = "Untitled")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Document name cannot be empty.", nameof(name));
        }

        Id = id ?? Guid.NewGuid();
        Name = name;

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Document id cannot be empty.", nameof(id));
        }
    }

    public Guid Id { get; }

    public string Name { get; }
}
