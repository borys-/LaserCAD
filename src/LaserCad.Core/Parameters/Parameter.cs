namespace LaserCad.Core.Parameters;

public sealed class Parameter
{
    public Parameter(ParameterId id, string name, ParameterType type, object? value)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Parameter name cannot be empty.", nameof(name));
        }

        Id = id;
        Name = name;
        Type = type;
        Value = value;
    }

    public ParameterId Id { get; }

    public string Name { get; }

    public ParameterType Type { get; }

    public object? Value { get; }
}
