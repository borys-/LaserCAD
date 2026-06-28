namespace LaserCad.Core.Parameters;

public sealed class Parameter
{
    public Parameter(ParameterId id, ParameterType type)
    {
        Id = id;
        Type = type;
    }

    public ParameterId Id { get; }

    public ParameterType Type { get; }
}
