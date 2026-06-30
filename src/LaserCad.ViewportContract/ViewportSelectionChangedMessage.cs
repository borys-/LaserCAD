namespace LaserCad.ViewportContract;

/// <summary>
/// Zdarzenie zmiany zaznaczenia wysylane przez viewport do desktop shell.
/// </summary>
public sealed record ViewportSelectionChangedMessage(IReadOnlyList<Guid> EntityIds);
