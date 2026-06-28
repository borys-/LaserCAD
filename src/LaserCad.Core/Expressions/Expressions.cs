using LaserCad.Core.Parameters;

namespace LaserCad.Core.Expressions;

/// <summary>
/// Fabryka ulatwiajaca jawne budowanie AST wyrazen parametrycznych.
/// Uzywaj jej zamiast recznego newowania wezlow, gdy chcesz czytelniej zapisac formule w kodzie.
/// </summary>
public static class Expressions
{
    /// <summary>
    /// Tworzy wyrazenie stale o podanej wartosci liczbowej.
    /// </summary>
    public static ConstantExpression Constant(double value)
    {
        return new ConstantExpression(value);
    }

    /// <summary>
    /// Tworzy referencje do parametru o podanym identyfikatorze.
    /// </summary>
    public static ParameterReferenceExpression Parameter(ParameterId parameterId)
    {
        return new ParameterReferenceExpression(parameterId);
    }

    /// <summary>
    /// Tworzy wyrazenie dodawania lewej i prawej strony.
    /// </summary>
    public static BinaryExpression Add(Expression left, Expression right)
    {
        return new BinaryExpression(left, BinaryOperator.Add, right);
    }

    /// <summary>
    /// Tworzy wyrazenie odejmowania prawej strony od lewej.
    /// </summary>
    public static BinaryExpression Subtract(Expression left, Expression right)
    {
        return new BinaryExpression(left, BinaryOperator.Subtract, right);
    }

    /// <summary>
    /// Tworzy wyrazenie mnozenia lewej i prawej strony.
    /// </summary>
    public static BinaryExpression Multiply(Expression left, Expression right)
    {
        return new BinaryExpression(left, BinaryOperator.Multiply, right);
    }

    /// <summary>
    /// Tworzy wyrazenie dzielenia lewej strony przez prawa.
    /// </summary>
    public static BinaryExpression Divide(Expression left, Expression right)
    {
        return new BinaryExpression(left, BinaryOperator.Divide, right);
    }
}
