using LaserCad.Core.Expressions;
using LaserCad.Core.Parameters;

namespace LaserCad.Tests.Core.Expressions;

public sealed class ParameterReferenceExpressionTests
{
    [Test]
    public void Constructor_ShouldStoreParameterId()
    {
        var parameterId = new ParameterId("Width");

        var expression = new ParameterReferenceExpression(parameterId);

        Assert.That(expression.ParameterId, Is.EqualTo(parameterId));
    }
}
