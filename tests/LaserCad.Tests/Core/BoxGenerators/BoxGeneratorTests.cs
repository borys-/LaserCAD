using LaserCad.Core.BoxGenerators;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.BoxGenerators;

public sealed class BoxGeneratorTests
{
    [Test]
    public void GenerateSketch_ShouldCreateSketch()
    {
        var generator = new BoxGenerator();

        Sketch sketch = generator.GenerateSketch(new BoxGeneratorOptions());

        Assert.That(sketch.Name, Is.EqualTo("Box generator"));
    }

    [Test]
    public void GenerateSketch_ShouldCreateFrontPanel()
    {
        var generator = new BoxGenerator();

        Sketch sketch = generator.GenerateSketch(new BoxGeneratorOptions());

        var frontPanel = (PolylineEntity)sketch.Entities[0];
        Assert.That(frontPanel.Polyline.IsClosed, Is.True);
        Assert.That(frontPanel.LayerName, Is.EqualTo(DefaultLayers.Cut.Name));
        AssertPanelBounds(frontPanel.Bounds, expectedWidth: 106.0, expectedHeight: 56.0);
    }

    private static void AssertPanelBounds(BoundingBox bounds, double expectedWidth, double expectedHeight)
    {
        Assert.That(bounds.MaxX - bounds.MinX, Is.EqualTo(expectedWidth).Within(0.000001));
        Assert.That(bounds.MaxY - bounds.MinY, Is.EqualTo(expectedHeight).Within(0.000001));
    }
}
