using LaserCad.Core.Production;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Production;

public sealed class SheetSizeTests
{
    [Test]
    public void Constructor_ShouldStoreWidth()
    {
        var sheetSize = new SheetSize(
            Length.FromMillimeters(600.0),
            Length.FromMillimeters(300.0));

        Assert.That(sheetSize.Width.Millimeters, Is.EqualTo(600.0));
    }

    [Test]
    public void Constructor_ShouldStoreHeight()
    {
        var sheetSize = new SheetSize(
            Length.FromMillimeters(600.0),
            Length.FromMillimeters(300.0));

        Assert.That(sheetSize.Height.Millimeters, Is.EqualTo(300.0));
    }

    [Test]
    public void Constructor_ShouldStoreMargin()
    {
        var sheetSize = new SheetSize(
            Length.FromMillimeters(600.0),
            Length.FromMillimeters(300.0),
            Length.FromMillimeters(10.0));

        Assert.That(sheetSize.Margin.Millimeters, Is.EqualTo(10.0));
    }
}
