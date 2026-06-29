using LaserCad.Core.Documents;

namespace LaserCad.Core.Commands;

/// <summary>
/// Przechowuje historie wykonanych komend i biezacy dokument CAD.
/// </summary>
public sealed class UndoRedoStack
{
    public UndoRedoStack(CadDocument document)
    {
        CurrentDocument = document ?? throw new ArgumentNullException(nameof(document));
    }

    /// <summary>
    /// Aktualna wersja dokumentu po wykonaniu historii komend.
    /// </summary>
    public CadDocument CurrentDocument { get; private set; }
}
