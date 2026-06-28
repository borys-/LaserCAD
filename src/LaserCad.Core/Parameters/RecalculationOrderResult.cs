namespace LaserCad.Core.Parameters;

public sealed class RecalculationOrderResult
{
    private RecalculationOrderResult(IReadOnlyList<ParameterId> order, string? error)
    {
        Order = order;
        Error = error;
    }

    public IReadOnlyList<ParameterId> Order { get; }

    public string? Error { get; }

    public bool IsSuccess => Error is null;

    public static RecalculationOrderResult Success(IReadOnlyList<ParameterId> order)
    {
        ArgumentNullException.ThrowIfNull(order);

        return new RecalculationOrderResult(order, null);
    }

    public static RecalculationOrderResult Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("Recalculation error cannot be empty.", nameof(error));
        }

        return new RecalculationOrderResult(Array.Empty<ParameterId>(), error);
    }
}
