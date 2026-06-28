namespace LaserCad.Core.Parameters;

/// <summary>
/// Stabilny identyfikator parametru.
/// Uzywaj go zamiast samego stringa przy referencjach, wyszukiwaniu i grafie zaleznosci.
/// </summary>
public readonly record struct ParameterId
{
    /// <summary>
    /// Tworzy identyfikator z niepustej wartosci tekstowej.
    /// Przekazuj nazwy stabilne w czasie, np. "Width" albo "MaterialThickness".
    /// </summary>
    public ParameterId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Parameter id cannot be empty.", nameof(value));
        }

        Value = value;
    }

    /// <summary>
    /// Tekstowa wartosc identyfikatora parametru.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Zwraca tekstowa wartosc identyfikatora, przydatna w komunikatach bledow i logach.
    /// </summary>
    public override string ToString()
    {
        return Value;
    }
}
