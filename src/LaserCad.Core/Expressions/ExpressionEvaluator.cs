using LaserCad.Core.Parameters;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.Expressions;

/// <summary>
/// Ewaluator wyrazen parametrycznych.
/// Uzywaj go z obiektem Expression i ParameterSet, aby otrzymac wynik liczbowy albo czytelny blad domenowy.
/// </summary>
public sealed class ExpressionEvaluator
{
    /// <summary>
    /// Ewaluje wyrazenie w kontekscie zestawu parametrow.
    /// Parametry typu Number sa czytane jako double, a Length jako milimetry.
    /// </summary>
    public ExpressionEvaluationResult Evaluate(Expression expression, ParameterSet parameters)
    {
        if (expression is null)
        {
            throw new ArgumentNullException(nameof(expression));
        }
        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        return expression switch
        {
            ConstantExpression constant => ExpressionEvaluationResult.Success(constant.Value),
            ParameterReferenceExpression reference => EvaluateReference(reference, parameters),
            BinaryExpression binary => EvaluateBinary(binary, parameters),
            _ => ExpressionEvaluationResult.Failure($"Unsupported expression type '{expression.GetType().Name}'.")
        };
    }

    /// <summary>
    /// Ewaluuje referencje do parametru, pobierajac wartosc z ParameterSet.
    /// Zwraca blad, jesli parametr nie istnieje albo ma typ nieobslugiwany przez MVP.
    /// </summary>
    private ExpressionEvaluationResult EvaluateReference(ParameterReferenceExpression expression, ParameterSet parameters)
    {
        var parameter = parameters.FindById(expression.ParameterId);

        if (parameter is null)
        {
            return ExpressionEvaluationResult.Failure($"Parameter '{expression.ParameterId}' was not found.");
        }

        return parameter.Value switch
        {
            double number => ExpressionEvaluationResult.Success(number),
            Length length => ExpressionEvaluationResult.Success(length.Millimeters),
            _ => ExpressionEvaluationResult.Failure($"Parameter '{expression.ParameterId}' has unsupported value type '{parameter.Type}'.")
        };
    }

    /// <summary>
    /// Ewaluuje operacje binarna, najpierw liczac lewa i prawa strone.
    /// Zwraca pierwszy napotkany blad albo wynik operacji arytmetycznej.
    /// </summary>
    private ExpressionEvaluationResult EvaluateBinary(BinaryExpression expression, ParameterSet parameters)
    {
        var left = Evaluate(expression.Left, parameters);
        if (!left.IsSuccess)
        {
            return left;
        }

        var right = Evaluate(expression.Right, parameters);
        if (!right.IsSuccess)
        {
            return right;
        }

        if (expression.OperatorKind == BinaryOperator.Divide && right.Value == 0.0)
        {
            return ExpressionEvaluationResult.Failure("Expression cannot be divided by zero.");
        }

        var result = expression.OperatorKind switch
        {
            BinaryOperator.Add => left.Value + right.Value,
            BinaryOperator.Subtract => left.Value - right.Value,
            BinaryOperator.Multiply => left.Value * right.Value,
            BinaryOperator.Divide => left.Value / right.Value,
            _ => double.NaN
        };

        return ExpressionEvaluationResult.Success(result);
    }
}
