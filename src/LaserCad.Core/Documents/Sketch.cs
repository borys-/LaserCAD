namespace LaserCad.Core.Documents;

public sealed class Sketch
{
    public Sketch(Guid? id = null, string name = "Sketch", IEnumerable<Entity>? entities = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Sketch name cannot be empty.", nameof(name));
        }

        Id = id ?? Guid.NewGuid();
        Name = name;
        Entities = entities?.ToArray() ?? Array.Empty<Entity>();

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Sketch id cannot be empty.", nameof(id));
        }

        if (Entities.Any(entity => entity is null))
        {
            throw new ArgumentException("Sketch entities cannot contain null values.", nameof(entities));
        }
    }

    public Guid Id { get; }

    public string Name { get; }

    public IReadOnlyList<Entity> Entities { get; }

    public Sketch AddEntity(Entity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new Sketch(Id, Name, Entities.Append(entity));
    }
}
