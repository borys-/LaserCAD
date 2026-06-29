using LaserCad.Core.Commands;

namespace LaserCad.Tests.Core.Commands;

public sealed class ICommandTests
{
    [Test]
    public void ICommand_ShouldBeInterface()
    {
        Assert.That(typeof(ICommand).IsInterface, Is.True);
    }

    [Test]
    public void ICommand_ShouldDeclareExecute()
    {
        var method = typeof(ICommand).GetMethod(nameof(ICommand.Execute));

        Assert.That(method, Is.Not.Null);
        Assert.That(method!.ReturnType.Name, Is.EqualTo("CadDocument"));
    }

    [Test]
    public void ICommand_ShouldDeclareUndo()
    {
        var method = typeof(ICommand).GetMethod(nameof(ICommand.Undo));

        Assert.That(method, Is.Not.Null);
        Assert.That(method!.ReturnType.Name, Is.EqualTo("CadDocument"));
    }
}
