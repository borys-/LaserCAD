namespace LaserCad.Core.Expressions;

public sealed class ExpressionEvaluationResult
{
    private ExpressionEvaluationResult(double value, string? error)
    {
        Value = value;
        Error = error;
    }

    public double Value { get; }

    public string? Error { get; }

    public bool IsSuccess => Error is null;

    public static ExpressionEvaluationResult Success(double value)
    {
        return new ExpressionEvaluationResult(value, null);
    }

    public static ExpressionEvaluationResult Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("Expression evaluation error cannot be empty.", nameof(error));
        }

        return new ExpressionEvaluationResult(0.0, error);
    }
}
