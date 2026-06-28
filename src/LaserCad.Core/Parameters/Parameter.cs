using LaserCad.Geometry.Units;

namespace LaserCad.Core.Parameters;

/// <summary>
/// Reprezentuje pojedynczy parametr domenowy dokumentu albo generatora.
/// Uzywaj go do przechowywania wartosci parametrycznych z typem, nazwa, jednostka wyswietlania i opcjonalnym zakresem.
/// </summary>
public sealed class Parameter
{
    /// <summary>
    /// Tworzy parametr i waliduje zgodnosc wartosci oraz zakresu z podanym ParameterType.
    /// Dla Length przekazuj Length, dla Number double, dla Boolean bool, a dla Text i Choice string.
    /// </summary>
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

        ValidateValueType(type, value, nameof(value));
        ValidateRangeType(type, minimumValue, nameof(minimumValue));
        ValidateRangeType(type, maximumValue, nameof(maximumValue));
        ValidateRangeOrder(type, minimumValue, maximumValue);
        ValidateValueRange(type, value, minimumValue, maximumValue);

        Id = id;
        Name = name;
        Type = type;
        Value = value;
        DisplayUnit = displayUnit;
        MinimumValue = minimumValue;
        MaximumValue = maximumValue;
    }

    /// <summary>
    /// Stabilny identyfikator parametru uzywany w wyrazeniach i grafie zaleznosci.
    /// </summary>
    public ParameterId Id { get; }

    /// <summary>
    /// Nazwa parametru czytelna dla uzytkownika.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Typ wartosci parametru, ktory steruje walidacja i sposobem uzycia.
    /// </summary>
    public ParameterType Type { get; }

    /// <summary>
    /// Aktualna wartosc parametru.
    /// Odczytuj ja zgodnie z Type albo przez mechanizmy wyrazen.
    /// </summary>
    public object? Value { get; }

    /// <summary>
    /// Opcjonalna jednostka prezentacji, np. "mm"; nie zmienia wewnetrznej wartosci.
    /// </summary>
    public string? DisplayUnit { get; }

    /// <summary>
    /// Opcjonalna minimalna wartosc parametru dla typow Length i Number.
    /// </summary>
    public object? MinimumValue { get; }

    /// <summary>
    /// Opcjonalna maksymalna wartosc parametru dla typow Length i Number.
    /// </summary>
    public object? MaximumValue { get; }

    /// <summary>
    /// Sprawdza, czy przekazana wartosc pasuje do typu parametru.
    /// Uzywane w konstruktorze, aby parametr nie powstal w niespojnym stanie.
    /// </summary>
    private static void ValidateValueType(ParameterType type, object? value, string parameterName)
    {
        if (!IsValueValidForType(type, value))
        {
            throw new ArgumentException($"Value is not valid for parameter type {type}.", parameterName);
        }
    }

    /// <summary>
    /// Sprawdza, czy wartosc zakresu jest dozwolona dla danego typu parametru.
    /// Zakres jest wspierany tylko dla Length i Number.
    /// </summary>
    private static void ValidateRangeType(ParameterType type, object? value, string parameterName)
    {
        if (value is null)
        {
            return;
        }

        if (type is not (ParameterType.Length or ParameterType.Number))
        {
            throw new ArgumentException($"Parameter type {type} does not support range values.", parameterName);
        }

        ValidateValueType(type, value, parameterName);
    }

    /// <summary>
    /// Zwraca informacje, czy wartosc ma typ zgodny z ParameterType.
    /// Uzywane jako wspolna reguła walidacji wartosci i zakresow.
    /// </summary>
    private static bool IsValueValidForType(ParameterType type, object? value)
    {
        return type switch
        {
            ParameterType.Length => value is Length,
            ParameterType.Number => value is double,
            ParameterType.Boolean => value is bool,
            ParameterType.Text => value is string,
            ParameterType.Choice => value is string,
            _ => false
        };
    }

    /// <summary>
    /// Sprawdza, czy minimum nie jest wieksze od maksimum.
    /// Uzywane tylko dla porownywalnych typow parametrow.
    /// </summary>
    private static void ValidateRangeOrder(ParameterType type, object? minimumValue, object? maximumValue)
    {
        if (minimumValue is null || maximumValue is null)
        {
            return;
        }

        if (Compare(type, minimumValue, maximumValue) > 0)
        {
            throw new ArgumentException("Minimum value cannot be greater than maximum value.", nameof(minimumValue));
        }
    }

    /// <summary>
    /// Sprawdza, czy wartosc parametru miesci sie w opcjonalnym zakresie minimum/maksimum.
    /// </summary>
    private static void ValidateValueRange(ParameterType type, object? value, object? minimumValue, object? maximumValue)
    {
        if (minimumValue is not null && Compare(type, value, minimumValue) < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Parameter value cannot be lower than minimum value.");
        }

        if (maximumValue is not null && Compare(type, value, maximumValue) > 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Parameter value cannot be greater than maximum value.");
        }
    }

    /// <summary>
    /// Porownuje dwie wartosci zakresu zgodnie z typem parametru.
    /// Obecnie wspiera Length i Number.
    /// </summary>
    private static int Compare(ParameterType type, object? left, object? right)
    {
        return type switch
        {
            ParameterType.Length => ((Length)left!).CompareTo((Length)right!),
            ParameterType.Number => ((double)left!).CompareTo((double)right!),
            _ => throw new InvalidOperationException($"Parameter type {type} does not support comparison.")
        };
    }
}
