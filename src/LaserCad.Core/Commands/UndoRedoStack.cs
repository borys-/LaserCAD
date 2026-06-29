using LaserCad.Core.Documents;

namespace LaserCad.Core.Commands;

/// <summary>
/// Przechowuje historie wykonanych komend i biezacy dokument CAD.
/// </summary>
public sealed class UndoRedoStack
{
    private readonly List<ICommand> _undoStack = new();
    private readonly List<ICommand> _redoStack = new();

    public UndoRedoStack(CadDocument document)
    {
        CurrentDocument = document ?? throw new ArgumentNullException(nameof(document));
    }

    /// <summary>
    /// Aktualna wersja dokumentu po wykonaniu historii komend.
    /// </summary>
    public CadDocument CurrentDocument { get; private set; }

    /// <summary>
    /// Liczba komend dostepnych do cofniecia.
    /// </summary>
    public int UndoCount => _undoStack.Count;

    /// <summary>
    /// Informuje, czy historia zawiera komende do cofniecia.
    /// </summary>
    public bool CanUndo => UndoCount > 0;

    /// <summary>
    /// Liczba komend dostepnych do ponowienia.
    /// </summary>
    public int RedoCount => _redoStack.Count;

    /// <summary>
    /// Informuje, czy historia zawiera komende do ponowienia.
    /// </summary>
    public bool CanRedo => RedoCount > 0;

    /// <summary>
    /// Wykonuje komende, aktualizuje dokument i dodaje komende do historii undo.
    /// </summary>
    public CadDocument Execute(ICommand command)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        CurrentDocument = command.Execute(CurrentDocument);
        _undoStack.Add(command);
        return CurrentDocument;
    }

    /// <summary>
    /// Cofa ostatnia wykonana komende i zwraca nowa wersje dokumentu.
    /// </summary>
    public CadDocument Undo()
    {
        if (!CanUndo)
        {
            throw new InvalidOperationException("Undo history is empty.");
        }

        var command = _undoStack[^1];
        _undoStack.RemoveAt(_undoStack.Count - 1);
        CurrentDocument = command.Undo(CurrentDocument);
        _redoStack.Add(command);
        return CurrentDocument;
    }

    /// <summary>
    /// Ponawia ostatnio cofnieta komende i zwraca nowa wersje dokumentu.
    /// </summary>
    public CadDocument Redo()
    {
        if (!CanRedo)
        {
            throw new InvalidOperationException("Redo history is empty.");
        }

        var command = _redoStack[^1];
        _redoStack.RemoveAt(_redoStack.Count - 1);
        CurrentDocument = command.Execute(CurrentDocument);
        _undoStack.Add(command);
        return CurrentDocument;
    }
}
