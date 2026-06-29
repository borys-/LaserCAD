using LaserCad.Core.Commands;
using LaserCad.Core.Documents;

namespace LaserCad.Core.FeatureTree;

/// <summary>
/// Wpis drzewa historii reprezentujacy pojedyncza operacje edycyjna.
/// </summary>
public sealed class EditOperationFeatureTreeItem : FeatureTreeItem
{
    public EditOperationFeatureTreeItem(
        ICommand command,
        Guid? id = null,
        string name = "Edit operation",
        bool isEnabled = true)
        : base(id, name, FeatureTreeItemKind.EditOperation, isEnabled)
    {
        if (command is null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        Command = command;
    }

    /// <summary>
    /// Komenda domenowa wykonywana podczas przebudowy drzewa historii.
    /// </summary>
    public ICommand Command { get; }

    public override FeatureTreeItem WithEnabled(bool isEnabled)
    {
        return new EditOperationFeatureTreeItem(Command, Id, Name, isEnabled);
    }

    public override CadDocument Apply(CadDocument document)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        return Command.Execute(document);
    }
}
