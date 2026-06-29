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

    [Test]
    public void GenerateSketch_ShouldCreateBackPanel()
    {
        var generator = new BoxGenerator();

        Sketch sketch = generator.GenerateSketch(new BoxGeneratorOptions());

        var backPanel = (PolylineEntity)sketch.Entities[1];
        Assert.That(backPanel.Polyline.IsClosed, Is.True);
        AssertPanelBounds(backPanel.Bounds, expectedWidth: 106.0, expectedHeight: 56.0);
    }

    [Test]
    public void GenerateSketch_ShouldCreateLeftPanel()
    {
        var generator = new BoxGenerator();

        Sketch sketch = generator.GenerateSketch(new BoxGeneratorOptions());

        var leftPanel = (PolylineEntity)sketch.Entities[2];
        Assert.That(leftPanel.Polyline.IsClosed, Is.True);
        AssertPanelBounds(leftPanel.Bounds, expectedWidth: 86.0, expectedHeight: 56.0);
    }

    [Test]
    public void GenerateSketch_ShouldCreateRightPanel()
    {
        var generator = new BoxGenerator();

        Sketch sketch = generator.GenerateSketch(new BoxGeneratorOptions());

        var rightPanel = (PolylineEntity)sketch.Entities[3];
        Assert.That(rightPanel.Polyline.IsClosed, Is.True);
        AssertPanelBounds(rightPanel.Bounds, expectedWidth: 86.0, expectedHeight: 56.0);
    }

    [Test]
    public void GenerateSketch_ShouldCreateBottomPanel()
    {
        var generator = new BoxGenerator();

        Sketch sketch = generator.GenerateSketch(new BoxGeneratorOptions());

        var bottomPanel = (PolylineEntity)sketch.Entities[4];
        Assert.That(bottomPanel.Polyline.IsClosed, Is.True);
        AssertPanelBounds(bottomPanel.Bounds, expectedWidth: 106.0, expectedHeight: 86.0);
    }

    [Test]
    public void GenerateSketch_WithLidType_ShouldCreateLidPanel()
    {
        var generator = new BoxGenerator();
        var options = new BoxGeneratorOptions(boxType: BoxGeneratorType.WithLid);

        Sketch sketch = generator.GenerateSketch(options);

        var lidPanel = (PolylineEntity)sketch.Entities[5];
        Assert.That(sketch.Entities, Has.Count.EqualTo(6));
        Assert.That(lidPanel.Polyline.IsClosed, Is.True);
        AssertPanelBounds(lidPanel.Bounds, expectedWidth: 106.0, expectedHeight: 86.0);
    }

    private static void AssertPanelBounds(BoundingBox bounds, double expectedWidth, double expectedHeight)
    {
        Assert.That(bounds.MaxX - bounds.MinX, Is.EqualTo(expectedWidth).Within(0.000001));
        Assert.That(bounds.MaxY - bounds.MinY, Is.EqualTo(expectedHeight).Within(0.000001));
    }
}
