namespace LaserCad.Core.Documents;

public abstract class Entity
{
    protected Entity(Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Entity id cannot be empty.", nameof(id));
        }
    }

    public Guid Id { get; }
}
