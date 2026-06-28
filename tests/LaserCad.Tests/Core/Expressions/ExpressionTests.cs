using LaserCad.Core.Expressions;

namespace LaserCad.Tests.Core.Expressions;

public sealed class ExpressionTests
{
    [Test]
    public void Expression_ShouldBeAbstract()
    {
        Assert.That(typeof(Expression).IsAbstract, Is.True);
    }
}
