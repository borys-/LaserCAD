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

    public Parameter? FindById(ParameterId id)
    {
        return _parameters.FirstOrDefault(parameter => parameter.Id == id);
    }

    public Parameter? FindByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Parameter name cannot be empty.", nameof(name));
        }

        return _parameters.FirstOrDefault(parameter => parameter.Name == name);
    }

    public ParameterSet Add(Parameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        return new ParameterSet(_parameters.Append(parameter));
    }

    public ParameterSet UpdateValue(ParameterId id, object? value)
    {
        var index = Array.FindIndex(_parameters, parameter => parameter.Id == id);

        if (index < 0)
        {
            throw new ArgumentException($"Parameter '{id}' was not found.", nameof(id));
        }

        var existingParameter = _parameters[index];
        var updatedParameter = new Parameter(
            existingParameter.Id,
            existingParameter.Name,
            existingParameter.Type,
            value,
            existingParameter.DisplayUnit,
            existingParameter.MinimumValue,
            existingParameter.MaximumValue);

        var updatedParameters = _parameters.ToArray();
        updatedParameters[index] = updatedParameter;

        return new ParameterSet(updatedParameters);
    }
}
