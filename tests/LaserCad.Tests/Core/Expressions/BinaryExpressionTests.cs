using LaserCad.Core.Expressions;

namespace LaserCad.Tests.Core.Expressions;

public sealed class BinaryExpressionTests
{
    [Test]
    public void Constructor_ForAdd_ShouldStoreOperandsAndOperator()
    {
        var left = new ConstantExpression(2.0);
        var right = new ConstantExpression(3.0);

        var expression = new BinaryExpression(left, BinaryOperator.Add, right);

        Assert.That(expression.Left, Is.SameAs(left));
        Assert.That(expression.OperatorKind, Is.EqualTo(BinaryOperator.Add));
        Assert.That(expression.Right, Is.SameAs(right));
    }

    [Test]
    public void Constructor_WithNullLeft_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new BinaryExpression(null!, BinaryOperator.Add, new ConstantExpression(1.0)));
    }

    [Test]
    public void Constructor_WithNullRight_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new BinaryExpression(new ConstantExpression(1.0), BinaryOperator.Add, null!));
    }

    [Test]
    public void Constructor_ForSubtract_ShouldStoreOperator()
    {
        var expression = new BinaryExpression(
            new ConstantExpression(5.0),
            BinaryOperator.Subtract,
            new ConstantExpression(2.0));

        Assert.That(expression.OperatorKind, Is.EqualTo(BinaryOperator.Subtract));
    }

    [Test]
    public void Constructor_ForMultiply_ShouldStoreOperator()
    {
        var expression = new BinaryExpression(
            new ConstantExpression(5.0),
            BinaryOperator.Multiply,
            new ConstantExpression(2.0));

        Assert.That(expression.OperatorKind, Is.EqualTo(BinaryOperator.Multiply));
    }

    [Test]
    public void Constructor_ForDivide_ShouldStoreOperator()
    {
        var expression = new BinaryExpression(
            new ConstantExpression(5.0),
            BinaryOperator.Divide,
            new ConstantExpression(2.0));

        Assert.That(expression.OperatorKind, Is.EqualTo(BinaryOperator.Divide));
    }
}
