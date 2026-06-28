using LaserCad.Core.Parameters;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.Expressions;

public sealed class ExpressionEvaluator
{
    public ExpressionEvaluationResult Evaluate(Expression expression, ParameterSet parameters)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(parameters);

        return expression switch
        {
            ConstantExpression constant => ExpressionEvaluationResult.Success(constant.Value),
            ParameterReferenceExpression reference => EvaluateReference(reference, parameters),
            BinaryExpression binary => EvaluateBinary(binary, parameters),
            _ => ExpressionEvaluationResult.Failure($"Unsupported expression type '{expression.GetType().Name}'.")
        };
    }

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
