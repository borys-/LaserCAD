namespace LaserCad.ViewportContract;

/// <summary>
/// Rodzaj komunikatu IPC miedzy desktop shell i Unity viewport.
/// </summary>
public enum ViewportMessageKind
{
    DocumentSnapshot,
    ViewCommand,
    SelectionChanged,
}
