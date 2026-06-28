using LaserCad.Geometry.Units;

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

    public ParameterId Id { get; }

    public string Name { get; }

    public ParameterType Type { get; }

    public object? Value { get; }

    public string? DisplayUnit { get; }

    public object? MinimumValue { get; }

    public object? MaximumValue { get; }

    private static void ValidateValueType(ParameterType type, object? value, string parameterName)
    {
        if (!IsValueValidForType(type, value))
        {
            throw new ArgumentException($"Value is not valid for parameter type {type}.", parameterName);
        }
    }

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
