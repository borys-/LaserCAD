using LaserCad.Core.Parameters;

namespace LaserCad.Core.Expressions;

public static class Expressions
{
    public static ConstantExpression Constant(double value)
    {
        return new ConstantExpression(value);
    }

    public static ParameterReferenceExpression Parameter(ParameterId parameterId)
    {
        return new ParameterReferenceExpression(parameterId);
    }

    public static BinaryExpression Add(Expression left, Expression right)
    {
        return new BinaryExpression(left, BinaryOperator.Add, right);
    }

    public static BinaryExpression Subtract(Expression left, Expression right)
    {
        return new BinaryExpression(left, BinaryOperator.Subtract, right);
    }

    public static BinaryExpression Multiply(Expression left, Expression right)
    {
        return new BinaryExpression(left, BinaryOperator.Multiply, right);
    }

    public static BinaryExpression Divide(Expression left, Expression right)
    {
        return new BinaryExpression(left, BinaryOperator.Divide, right);
    }
}
