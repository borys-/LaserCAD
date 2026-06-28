namespace LaserCad.Core.Parameters;

public readonly record struct ParameterId
{
    public ParameterId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Parameter id cannot be empty.", nameof(value));
        }

        Value = value;
    }

    public string Value { get; }

    public override string ToString()
    {
        return Value;
    }
}
