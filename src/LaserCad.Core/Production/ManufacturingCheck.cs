namespace LaserCad.Core.Production;

/// <summary>
/// Pojedynczy wynik kontroli przygotowania dokumentu do produkcji.
/// </summary>
public sealed class ManufacturingCheck
{
    /// <summary>
    /// Tworzy wynik kontroli produkcyjnej.
    /// </summary>
    public ManufacturingCheck(string code, string message, Guid? entityId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Manufacturing check code cannot be empty.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Manufacturing check message cannot be empty.", nameof(message));
        }

        Code = code;
        Message = message;
        EntityId = entityId;
    }

    /// <summary>
    /// Stabilny kod reguly, ktora zglosila wynik.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Czytelny opis problemu albo sugestii.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Opcjonalny identyfikator encji powiazanej z wynikiem kontroli.
    /// </summary>
    public Guid? EntityId { get; }
}
