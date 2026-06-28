namespace LaserCad.Core.Documents;

public sealed class GeneratorInstance
{
    public GeneratorInstance(Guid? id = null, string generatorType = "Generator")
    {
        if (string.IsNullOrWhiteSpace(generatorType))
        {
            throw new ArgumentException("Generator type cannot be empty.", nameof(generatorType));
        }

        Id = id ?? Guid.NewGuid();
        GeneratorType = generatorType;

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Generator id cannot be empty.", nameof(id));
        }
    }

    public Guid Id { get; }

    public string GeneratorType { get; }
}
