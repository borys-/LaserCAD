namespace LaserCad.Core.Expressions;

/// <summary>
/// Wyrazenie binarne skladajace lewa i prawa strone za pomoca operatora arytmetycznego.
/// Uzywaj go do budowania AST dla dodawania, odejmowania, mnozenia i dzielenia.
/// </summary>
public sealed class BinaryExpression : Expression
{
    /// <summary>
    /// Tworzy operacje binarna z dwoch wyrazen.
    /// Lewa i prawa strona nie moga byc null.
    /// </summary>
    public BinaryExpression(Expression left, BinaryOperator operatorKind, Expression right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        Left = left;
        OperatorKind = operatorKind;
        Right = right;
    }

    /// <summary>
    /// Lewa strona operacji.
    /// </summary>
    public Expression Left { get; }

    /// <summary>
    /// Operator arytmetyczny wykonywany miedzy stronami.
    /// </summary>
    public BinaryOperator OperatorKind { get; }

    /// <summary>
    /// Prawa strona operacji.
    /// </summary>
    public Expression Right { get; }
}
