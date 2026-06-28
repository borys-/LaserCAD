namespace LaserCad.Core.Parameters;

/// <summary>
/// Niemutowalna kolekcja parametrow.
/// Uzywaj jej do przechowywania parametrow dokumentu albo generatora oraz do wyszukiwania po id lub nazwie.
/// </summary>
public sealed class ParameterSet
{
    private readonly Parameter[] _parameters;

    /// <summary>
    /// Tworzy pusty zestaw parametrow.
    /// </summary>
    public ParameterSet()
        : this(Array.Empty<Parameter>())
    {
    }

    /// <summary>
    /// Tworzy zestaw z podanej sekwencji parametrow.
    /// Przydatne przy odczycie dokumentu albo budowaniu zestawu w testach.
    /// </summary>
    public ParameterSet(IEnumerable<Parameter> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        _parameters = parameters.ToArray();

        if (_parameters.Any(parameter => parameter is null))
        {
            throw new ArgumentException("Parameter set cannot contain null parameters.", nameof(parameters));
        }
    }

    /// <summary>
    /// Wszystkie parametry w zestawie w kolejnosci dodania.
    /// </summary>
    public IReadOnlyList<Parameter> Parameters => _parameters;

    /// <summary>
    /// Wyszukuje parametr po stabilnym identyfikatorze.
    /// Zwraca null, jesli parametr nie istnieje.
    /// </summary>
    public Parameter? FindById(ParameterId id)
    {
        return _parameters.FirstOrDefault(parameter => parameter.Id == id);
    }

    /// <summary>
    /// Wyszukuje parametr po nazwie uzytkowej.
    /// Zwraca null, jesli parametr nie istnieje.
    /// </summary>
    public Parameter? FindByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Parameter name cannot be empty.", nameof(name));
        }

        return _parameters.FirstOrDefault(parameter => parameter.Name == name);
    }

    /// <summary>
    /// Zwraca nowy zestaw z dodanym parametrem.
    /// Oryginalny zestaw pozostaje bez zmian.
    /// </summary>
    public ParameterSet Add(Parameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        return new ParameterSet(_parameters.Append(parameter));
    }

    /// <summary>
    /// Zwraca nowy zestaw z podmieniona wartoscia wskazanego parametru.
    /// Nowa wartosc przechodzi te sama walidacje typu i zakresu co przy tworzeniu parametru.
    /// </summary>
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
