namespace LaserCad.Core.Expressions;

public sealed class ConstantExpression : Expression
{
    public ConstantExpression(double value)
    {
        Value = value;
    }

    public double Value { get; }
}
