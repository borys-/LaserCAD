using LaserCad.Core.Expressions;
using LaserCad.Core.Parameters;
using ExpressionFactory = LaserCad.Core.Expressions.Expressions;

namespace LaserCad.Tests.Core.Expressions;

public sealed class ExpressionsTests
{
    [Test]
    public void FactoryMethods_ShouldBuildExplicitAst()
    {
        var expression = ExpressionFactory.Subtract(
            ExpressionFactory.Parameter(new ParameterId("Width")),
            ExpressionFactory.Multiply(
                ExpressionFactory.Constant(2.0),
                ExpressionFactory.Parameter(new ParameterId("MaterialThickness"))));

        Assert.That(expression.OperatorKind, Is.EqualTo(BinaryOperator.Subtract));
        Assert.That(expression.Left, Is.TypeOf<ParameterReferenceExpression>());
        Assert.That(expression.Right, Is.TypeOf<BinaryExpression>());
    }
}
