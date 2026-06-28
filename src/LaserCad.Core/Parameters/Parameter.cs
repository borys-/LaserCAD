namespace LaserCad.Core.Parameters;

public sealed class Parameter
{
    public Parameter(
        ParameterId id,
        string name,
        ParameterType type,
        object? value,
        string? displayUnit = null,
        object? minimumValue = null,
        object? maximumValue = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Parameter name cannot be empty.", nameof(name));
        }

        Id = id;
        Name = name;
        Type = type;
        Value = value;
        DisplayUnit = displayUnit;
        MinimumValue = minimumValue;
        MaximumValue = maximumValue;
    }

    public ParameterId Id { get; }

    public string Name { get; }

    public ParameterType Type { get; }

    public object? Value { get; }

    public string? DisplayUnit { get; }

    public object? MinimumValue { get; }

    public object? MaximumValue { get; }
}
