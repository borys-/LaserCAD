namespace LaserCad.Core.Documents;

public sealed class CadDocument
{
    public CadDocument(Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Document id cannot be empty.", nameof(id));
        }
    }

    public Guid Id { get; }
}
