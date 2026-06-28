namespace LaserCad.Core.Expressions;

public sealed class BinaryExpression : Expression
{
    public BinaryExpression(Expression left, BinaryOperator operatorKind, Expression right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        Left = left;
        OperatorKind = operatorKind;
        Right = right;
    }

    public Expression Left { get; }

    public BinaryOperator OperatorKind { get; }

    public Expression Right { get; }
}
