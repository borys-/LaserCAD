using LaserCad.Core.Documents;

namespace LaserCad.Core.Commands;

/// <summary>
/// Laczy wiele komend w jedna odwracalna operacje historii.
/// </summary>
public sealed class CommandGroup : ICommand
{
    public CommandGroup(IEnumerable<ICommand> commands)
    {
        if (commands is null)
        {
            throw new ArgumentNullException(nameof(commands));
        }

        Commands = commands.ToArray();

        if (Commands.Count == 0)
        {
            throw new ArgumentException("Command group cannot be empty.", nameof(commands));
        }

        if (Commands.Any(command => command is null))
        {
            throw new ArgumentException("Command group cannot contain null commands.", nameof(commands));
        }
    }

    /// <summary>
    /// Komendy wykonywane w kolejnosci dodania.
    /// </summary>
    public IReadOnlyList<ICommand> Commands { get; }

    public CadDocument Execute(CadDocument document)
    {
        var current = document ?? throw new ArgumentNullException(nameof(document));

        foreach (var command in Commands)
        {
            current = command.Execute(current);
        }

        return current;
    }

    public CadDocument Undo(CadDocument document)
    {
        var current = document ?? throw new ArgumentNullException(nameof(document));

        for (var index = Commands.Count - 1; index >= 0; index--)
        {
            current = Commands[index].Undo(current);
        }

        return current;
    }
}
