namespace LaserCad.Core.Expressions;

/// <summary>
/// Wyrazenie stale zwracajace zawsze te sama wartosc liczbowa.
/// Uzywaj go w AST dla literalow, np. liczby 2 w wyrazeniu Width - 2 * MaterialThickness.
/// </summary>
public sealed class ConstantExpression : Expression
{
    /// <summary>
    /// Tworzy stala liczbową.
    /// </summary>
    public ConstantExpression(double value)
    {
        Value = value;
    }

    /// <summary>
    /// Wartosc liczbowa zwracana przez to wyrazenie.
    /// </summary>
    public double Value { get; }
}
