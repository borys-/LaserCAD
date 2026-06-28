namespace LaserCad.Core.Parameters;

public sealed class ParameterSet
{
    private readonly Parameter[] _parameters;

    public ParameterSet()
        : this(Array.Empty<Parameter>())
    {
    }

    public ParameterSet(IEnumerable<Parameter> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        _parameters = parameters.ToArray();

        if (_parameters.Any(parameter => parameter is null))
        {
            throw new ArgumentException("Parameter set cannot contain null parameters.", nameof(parameters));
        }
    }

    public IReadOnlyList<Parameter> Parameters => _parameters;

    public ParameterSet Add(Parameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        return new ParameterSet(_parameters.Append(parameter));
    }
}
