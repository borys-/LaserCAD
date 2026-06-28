namespace LaserCad.Core.Expressions;

/// <summary>
/// Wynik ewaluacji wyrazenia parametrycznego.
/// Uzywaj IsSuccess, aby sprawdzic, czy Value zawiera wynik, czy Error opisuje problem domenowy.
/// </summary>
public sealed class ExpressionEvaluationResult
{
    /// <summary>
    /// Tworzy wewnetrzna reprezentacje wyniku.
    /// Uzywaj publicznych metod Success i Failure zamiast bezposredniego konstruktora.
    /// </summary>
    private ExpressionEvaluationResult(double value, string? error)
    {
        Value = value;
        Error = error;
    }

    /// <summary>
    /// Wartosc wyrazenia, gdy IsSuccess jest true.
    /// </summary>
    public double Value { get; }

    /// <summary>
    /// Komunikat bledu, gdy ewaluacja sie nie powiodla.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Informuje, czy ewaluacja zakonczyla sie poprawnie.
    /// </summary>
    public bool IsSuccess => Error is null;

    /// <summary>
    /// Tworzy poprawny wynik ewaluacji z wartoscia liczbową.
    /// </summary>
    public static ExpressionEvaluationResult Success(double value)
    {
        return new ExpressionEvaluationResult(value, null);
    }

    /// <summary>
    /// Tworzy wynik bledu z czytelnym komunikatem.
    /// Uzywaj dla przewidywalnych bledow, np. brakujacego parametru albo dzielenia przez zero.
    /// </summary>
    public static ExpressionEvaluationResult Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("Expression evaluation error cannot be empty.", nameof(error));
        }

        return new ExpressionEvaluationResult(0.0, error);
    }
}
