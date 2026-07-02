using LaserCad.Core.Documents;
using LaserCad.Core.MaterialModel;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.MaterialModel;

public sealed class FlatPartTests
{
    [Test]
    public void Constructor_ShouldPreserveProductionContract()
    {
        var outer = CreateRectangle(20.0, 10.0);
        var inner = CreateRectangle(4.0, 4.0);

        var part = new FlatPart("Front", outer, new[] { inner }, DefaultLayers.All);

        Assert.That(part.Name, Is.EqualTo("Front"));
        Assert.That(part.OuterContour, Is.SameAs(outer));
        Assert.That(part.InnerContours, Is.EqualTo(new[] { inner }));
        Assert.That(part.Layers.Select(layer => layer.Role), Is.EquivalentTo(new[]
        {
            LayerRole.Cut,
            LayerRole.Engrave,
            LayerRole.Score,
            LayerRole.Ignore
        }));
        Assert.That(part.Quantity, Is.EqualTo(1));
        Assert.That(part.SourceNames, Is.EqualTo(new[] { "Front" }));
    }

    [Test]
    public void Constructor_WithInvalidQuantity_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new FlatPart("Front", CreateRectangle(20.0, 10.0), quantity: 0));
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
