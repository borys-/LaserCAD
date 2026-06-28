using LaserCad.Core.Expressions;

namespace LaserCad.Tests.Core.Expressions;

public sealed class ConstantExpressionTests
{
    [Test]
    public void Constructor_ShouldStoreValue()
    {
        var expression = new ConstantExpression(12.5);

        Assert.That(expression.Value, Is.EqualTo(12.5));
    }
}
