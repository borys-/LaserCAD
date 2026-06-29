using LaserCad.Core.Commands;

namespace LaserCad.Tests.Core.Commands;

public sealed class ICommandTests
{
    [Test]
    public void ICommand_ShouldBeInterface()
    {
        Assert.That(typeof(ICommand).IsInterface, Is.True);
    }
}
