namespace LaserCad.Core.Commands;

using LaserCad.Core.Documents;

/// <summary>
/// Reprezentuje odwracalna komenda edycyjna wykonywana na dokumencie CAD.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Wykonuje komende na dokumencie i zwraca nowa wersje dokumentu.
    /// </summary>
    CadDocument Execute(CadDocument document);

    /// <summary>
    /// Cofa komende na dokumencie i zwraca nowa wersje dokumentu.
    /// </summary>
    CadDocument Undo(CadDocument document);
}
