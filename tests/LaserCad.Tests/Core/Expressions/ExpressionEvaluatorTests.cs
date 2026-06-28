using LaserCad.Core.Expressions;
using LaserCad.Core.Parameters;
using LaserCad.Geometry.Units;
using ExpressionFactory = LaserCad.Core.Expressions.Expressions;

namespace LaserCad.Tests.Core.Expressions;

public sealed class ExpressionEvaluatorTests
{
    [Test]
    public void Evaluate_WithConstantExpression_ShouldReturnValue()
    {
        var evaluator = new ExpressionEvaluator();

        var result = evaluator.Evaluate(new ConstantExpression(12.5), new ParameterSet());

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(12.5));
    }

    [Test]
    public void Evaluate_WithNumberParameterReference_ShouldReturnParameterValue()
    {
        var evaluator = new ExpressionEvaluator();
        var parameters = new ParameterSet([
            new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 120.0)
        ]);

        var result = evaluator.Evaluate(new ParameterReferenceExpression(new ParameterId("Width")), parameters);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(120.0));
    }

    [Test]
    public void Evaluate_WithLengthParameterReference_ShouldReturnMillimeters()
    {
        var evaluator = new ExpressionEvaluator();
        var parameters = new ParameterSet([
            new Parameter(new ParameterId("MaterialThickness"), "Material thickness", ParameterType.Length, Length.FromMillimeters(3.0))
        ]);

        var result = evaluator.Evaluate(new ParameterReferenceExpression(new ParameterId("MaterialThickness")), parameters);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(3.0));
    }

    [Test]
    public void Evaluate_WithBinaryOperators_ShouldReturnResult()
    {
        var evaluator = new ExpressionEvaluator();
        var expression = new BinaryExpression(
            new BinaryExpression(new ConstantExpression(10.0), BinaryOperator.Subtract, new ConstantExpression(2.0)),
            BinaryOperator.Multiply,
            new BinaryExpression(new ConstantExpression(8.0), BinaryOperator.Divide, new ConstantExpression(4.0)));

        var result = evaluator.Evaluate(expression, new ParameterSet());

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(16.0));
    }

    [Test]
    public void Evaluate_WithMissingParameter_ShouldReturnReadableError()
    {
        var evaluator = new ExpressionEvaluator();

        var result = evaluator.Evaluate(new ParameterReferenceExpression(new ParameterId("Width")), new ParameterSet());

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Parameter 'Width' was not found."));
    }

    [Test]
    public void Evaluate_WithDivisionByZero_ShouldReturnReadableError()
    {
        var evaluator = new ExpressionEvaluator();
        var expression = new BinaryExpression(
            new ConstantExpression(10.0),
            BinaryOperator.Divide,
            new ConstantExpression(0.0));

        var result = evaluator.Evaluate(expression, new ParameterSet());

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("Expression cannot be divided by zero."));
    }

    [Test]
    public void Evaluate_WidthMinusTwoTimesMaterialThickness_ShouldReturnInnerWidth()
    {
        var evaluator = new ExpressionEvaluator();
        var parameters = new ParameterSet([
            new Parameter(new ParameterId("Width"), "Width", ParameterType.Length, Length.FromMillimeters(100.0)),
            new Parameter(new ParameterId("MaterialThickness"), "Material thickness", ParameterType.Length, Length.FromMillimeters(3.0))
        ]);
        var expression = ExpressionFactory.Subtract(
            ExpressionFactory.Parameter(new ParameterId("Width")),
            ExpressionFactory.Multiply(
                ExpressionFactory.Constant(2.0),
                ExpressionFactory.Parameter(new ParameterId("MaterialThickness"))));

        var result = evaluator.Evaluate(expression, parameters);

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(94.0));
    }
}
