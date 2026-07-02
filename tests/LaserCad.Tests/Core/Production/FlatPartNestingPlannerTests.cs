using LaserCad.Core.MaterialModel;
using LaserCad.Core.Production;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Production;

public sealed class FlatPartNestingPlannerTests
{
    [Test]
    public void CreateNestingItems_ShouldUseFlatPartBoundsAndQuantity()
    {
        var part = new FlatPart("Side", CreateRectangle(30.0, 20.0), quantity: 2);
        var planner = new FlatPartNestingPlanner();

        var items = planner.CreateNestingItems(new[] { part });

        Assert.That(items, Has.Count.EqualTo(2));
        Assert.That(items[0].Name, Is.EqualTo("Side #1"));
        Assert.That(items[0].Width.Millimeters, Is.EqualTo(30.0));
        Assert.That(items[0].Height.Millimeters, Is.EqualTo(20.0));
    }

    [Test]
    public void NestSingleSheet_ShouldPlaceFlatPartsWithSpacingAndMargin()
    {
        var sheet = new SheetSize(Length.FromMillimeters(100.0), Length.FromMillimeters(100.0), Length.FromMillimeters(5.0));
        var options = new NestingOptions(spacing: Length.FromMillimeters(5.0));
        var parts = new[]
        {
            new FlatPart("Front", CreateRectangle(30.0, 20.0)),
            new FlatPart("Back", CreateRectangle(30.0, 20.0))
        };
        var planner = new FlatPartNestingPlanner();

        var result = planner.NestSingleSheet(sheet, parts, options);

        Assert.That(result.Parts, Has.Count.EqualTo(2));
        Assert.That(result.Parts[0].X.Millimeters, Is.EqualTo(5.0));
        Assert.That(result.Parts[1].X.Millimeters, Is.EqualTo(40.0));
    }

    [Test]
    public void NestSingleSheet_WithRotationAllowed_ShouldRotateFlatPart()
    {
        var sheet = new SheetSize(Length.FromMillimeters(50.0), Length.FromMillimeters(120.0), Length.FromMillimeters(5.0));
        var options = new NestingOptions(allowRotation: true);
        var part = new FlatPart("Wide", CreateRectangle(80.0, 30.0));
        var planner = new FlatPartNestingPlanner();

        var result = planner.NestSingleSheet(sheet, new[] { part }, options);

        Assert.That(result.Parts[0].IsRotated, Is.True);
        Assert.That(result.Parts[0].Width.Millimeters, Is.EqualTo(30.0));
        Assert.That(result.Parts[0].Height.Millimeters, Is.EqualTo(80.0));
    }

    [Test]
    public void NestMultipleSheets_ShouldCreateNextSheetWhenPartsDoNotFitTogether()
    {
        var sheet = new SheetSize(Length.FromMillimeters(50.0), Length.FromMillimeters(50.0), Length.FromMillimeters(5.0));
        var options = new NestingOptions(spacing: Length.FromMillimeters(5.0));
        var parts = new[]
        {
            new FlatPart("A", CreateRectangle(35.0, 35.0)),
            new FlatPart("B", CreateRectangle(35.0, 35.0))
        };
        var planner = new FlatPartNestingPlanner();

        var sheets = planner.NestMultipleSheets(sheet, parts, options);

        Assert.That(sheets, Has.Count.EqualTo(2));
        Assert.That(sheets[0].Parts.Single().Item.Name, Is.EqualTo("A"));
        Assert.That(sheets[1].Parts.Single().Item.Name, Is.EqualTo("B"));
    }

    [Test]
    public void NestMultipleSheets_WithOversizedPart_ShouldReturnReadableError()
    {
        var sheet = new SheetSize(Length.FromMillimeters(50.0), Length.FromMillimeters(50.0), Length.FromMillimeters(5.0));
        var part = new FlatPart("Too big", CreateRectangle(80.0, 80.0));
        var planner = new FlatPartNestingPlanner();

        var exception = Assert.Throws<InvalidOperationException>(() =>
            planner.NestMultipleSheets(sheet, new[] { part }));

        Assert.That(exception!.Message, Does.Contain("Too big"));
        Assert.That(exception.Message, Does.Contain("empty sheet"));
    }

    private static Polygon2D CreateRectangle(double width, double height)
    {
        return new Polygon2D(new[]
        {
            new Point2D(0.0, 0.0),
            new Point2D(width, 0.0),
            new Point2D(width, height),
            new Point2D(0.0, height)
        });
    }
}
