using LaserCad.Core.Production;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Production;

public sealed class RowNestingPlannerTests
{
    [Test]
    public void Nest_ShouldPlaceItemsInRows()
    {
        var sheet = new SheetSize(
            Length.FromMillimeters(100.0),
            Length.FromMillimeters(100.0),
            Length.FromMillimeters(5.0));
        var options = new NestingOptions(spacing: Length.FromMillimeters(5.0));
        var items = new[]
        {
            new NestingItem("A", Length.FromMillimeters(40.0), Length.FromMillimeters(10.0)),
            new NestingItem("B", Length.FromMillimeters(40.0), Length.FromMillimeters(20.0)),
            new NestingItem("C", Length.FromMillimeters(30.0), Length.FromMillimeters(15.0)),
        };

        NestingResult result = new RowNestingPlanner().Nest(sheet, items, options);

        Assert.That(result.Parts, Has.Count.EqualTo(3));
        Assert.That(result.Parts[0].X.Millimeters, Is.EqualTo(5.0));
        Assert.That(result.Parts[0].Y.Millimeters, Is.EqualTo(5.0));
        Assert.That(result.Parts[1].X.Millimeters, Is.EqualTo(50.0));
        Assert.That(result.Parts[1].Y.Millimeters, Is.EqualTo(5.0));
        Assert.That(result.Parts[2].X.Millimeters, Is.EqualTo(5.0));
        Assert.That(result.Parts[2].Y.Millimeters, Is.EqualTo(30.0));
    }

    [Test]
    public void Nest_WithRotationAllowed_ShouldRotateWideItem()
    {
        var sheet = new SheetSize(
            Length.FromMillimeters(50.0),
            Length.FromMillimeters(120.0),
            Length.FromMillimeters(5.0));
        var item = new NestingItem("Wide", Length.FromMillimeters(80.0), Length.FromMillimeters(30.0));

        NestingResult result = new RowNestingPlanner().Nest(
            sheet,
            new[] { item },
            new NestingOptions(allowRotation: true));

        Assert.That(result.Parts[0].IsRotated, Is.True);
        Assert.That(result.Parts[0].Width.Millimeters, Is.EqualTo(30.0));
        Assert.That(result.Parts[0].Height.Millimeters, Is.EqualTo(80.0));
    }

    [Test]
    public void Nest_WithSeveralRectangles_ShouldKeepEveryRectangleInsideSheet()
    {
        var sheet = new SheetSize(
            Length.FromMillimeters(120.0),
            Length.FromMillimeters(80.0),
            Length.FromMillimeters(5.0));
        var options = new NestingOptions(spacing: Length.FromMillimeters(5.0));
        var items = new[]
        {
            new NestingItem("Front", Length.FromMillimeters(30.0), Length.FromMillimeters(20.0)),
            new NestingItem("Back", Length.FromMillimeters(30.0), Length.FromMillimeters(20.0)),
            new NestingItem("Side", Length.FromMillimeters(20.0), Length.FromMillimeters(20.0)),
            new NestingItem("Bottom", Length.FromMillimeters(40.0), Length.FromMillimeters(25.0)),
        };

        NestingResult result = new RowNestingPlanner().Nest(sheet, items, options);

        Assert.That(result.Parts, Has.Count.EqualTo(4));
        Assert.That(result.Parts, Is.All.Matches<NestedPart>(part =>
            part.X.Millimeters >= sheet.Margin.Millimeters &&
            part.Y.Millimeters >= sheet.Margin.Millimeters &&
            part.X.Millimeters + part.Width.Millimeters <= sheet.Width.Millimeters - sheet.Margin.Millimeters &&
            part.Y.Millimeters + part.Height.Millimeters <= sheet.Height.Millimeters - sheet.Margin.Millimeters));
    }
}
