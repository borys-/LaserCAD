namespace LaserCad.Core.Parameters;

/// <summary>
/// Wynik wyznaczania kolejnosci przeliczania parametrow.
/// Uzywaj IsSuccess, aby rozroznic poprawna kolejnosc od bledu domenowego, np. cyklu.
/// </summary>
public sealed class RecalculationOrderResult
{
    /// <summary>
    /// Tworzy wewnetrzna reprezentacje wyniku przeliczania.
    /// Uzywaj publicznych metod Success i Failure zamiast bezposredniego konstruktora.
    /// </summary>
    private RecalculationOrderResult(IReadOnlyList<ParameterId> order, string? error)
    {
        Order = order;
        Error = error;
    }

    /// <summary>
    /// Kolejnosc parametrow do przeliczenia, gdy wynik jest poprawny.
    /// </summary>
    public IReadOnlyList<ParameterId> Order { get; }

    /// <summary>
    /// Komunikat bledu, gdy nie da sie wyznaczyc kolejnosci.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Informuje, czy wynik zawiera poprawna kolejnosc zamiast bledu.
    /// </summary>
    public bool IsSuccess => Error is null;

    /// <summary>
    /// Tworzy poprawny wynik z gotowa kolejnoscia przeliczania.
    /// </summary>
    public static RecalculationOrderResult Success(IReadOnlyList<ParameterId> order)
    {
        ArgumentNullException.ThrowIfNull(order);

        return new RecalculationOrderResult(order, null);
    }

    /// <summary>
    /// Tworzy wynik bledu z czytelnym komunikatem.
    /// Uzywaj dla przewidywalnych problemow domenowych, takich jak cykl zaleznosci.
    /// </summary>
    public static RecalculationOrderResult Failure(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            throw new ArgumentException("Recalculation error cannot be empty.", nameof(error));
        }

        return new RecalculationOrderResult(Array.Empty<ParameterId>(), error);
    }
}
