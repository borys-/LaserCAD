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
}
