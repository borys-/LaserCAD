using LaserCad.Core.Parameters;

namespace LaserCad.Core.Documents;

public sealed class GeneratorInstance
{
    public GeneratorInstance(Guid? id = null, string generatorType = "Generator", ParameterSet? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(generatorType))
        {
            throw new ArgumentException("Generator type cannot be empty.", nameof(generatorType));
        }

        Id = id ?? Guid.NewGuid();
        GeneratorType = generatorType;
        Parameters = parameters ?? new ParameterSet();

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Generator id cannot be empty.", nameof(id));
        }
    }

    public Guid Id { get; }

    public string GeneratorType { get; }

    public ParameterSet Parameters { get; }

    public GeneratorInstance AddParameter(Parameter parameter)
    {
        return new GeneratorInstance(Id, GeneratorType, Parameters.Add(parameter));
    }
}
