namespace LaserCad.Core.Documents;

public sealed class Sketch
{
    public Sketch(Guid? id = null, string name = "Sketch")
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Sketch name cannot be empty.", nameof(name));
        }

        Id = id ?? Guid.NewGuid();
        Name = name;

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Sketch id cannot be empty.", nameof(id));
        }
    }

    public Guid Id { get; }

    public string Name { get; }
}
