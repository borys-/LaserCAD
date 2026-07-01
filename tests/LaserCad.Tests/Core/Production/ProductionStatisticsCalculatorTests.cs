using LaserCad.Core.Production;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Production;

public sealed class ProductionStatisticsCalculatorTests
{
    [Test]
    public void Calculate_ShouldReturnMaterialUsageRatio()
    {
        var sheet = new SheetSize(
            Length.FromMillimeters(100.0),
            Length.FromMillimeters(100.0));
        var result = new NestingResult(new[]
        {
            new NestedPart(
                new NestingItem("A", Length.FromMillimeters(20.0), Length.FromMillimeters(10.0)),
                Length.FromMillimeters(0.0),
                Length.FromMillimeters(0.0),
                Length.FromMillimeters(20.0),
                Length.FromMillimeters(10.0),
                isRotated: false),
        });

        ProductionStatistics statistics = new ProductionStatisticsCalculator().Calculate(sheet, result);

        Assert.That(statistics.MaterialUsageRatio, Is.EqualTo(0.02).Within(0.000001));
    }

    [Test]
    public void Calculate_ShouldReturnCuttingLength()
    {
        var sheet = new SheetSize(
            Length.FromMillimeters(100.0),
            Length.FromMillimeters(100.0));
        var result = new NestingResult(new[]
        {
            new NestedPart(
                new NestingItem("A", Length.FromMillimeters(20.0), Length.FromMillimeters(10.0)),
                Length.FromMillimeters(0.0),
                Length.FromMillimeters(0.0),
                Length.FromMillimeters(20.0),
                Length.FromMillimeters(10.0),
                isRotated: false),
        });

        ProductionStatistics statistics = new ProductionStatisticsCalculator().Calculate(sheet, result);

        Assert.That(statistics.CuttingLengthMillimeters, Is.EqualTo(60.0));
    }
}
