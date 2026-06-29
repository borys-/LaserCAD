using LaserCad.Core.BoxGenerators;
using LaserCad.Core.Documents;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

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

    [Test]
    public void GenerateSketch_ShouldAddFingerJointsToPanelEdges()
    {
        var generator = new BoxGenerator();

        Sketch sketch = generator.GenerateSketch(new BoxGeneratorOptions());

        var frontPanel = (PolylineEntity)sketch.Entities[0];
        Assert.That(frontPanel.Polyline.Points, Has.Count.GreaterThan(12));
        Assert.That(frontPanel.Bounds.MinX, Is.LessThan(0.0));
        Assert.That(frontPanel.Bounds.MaxX, Is.GreaterThan(100.0));
    }

    [Test]
    public void GenerateSketch_ShouldLayOutPanelsOnPlane()
    {
        var generator = new BoxGenerator();

        Sketch sketch = generator.GenerateSketch(new BoxGeneratorOptions());

        var frontPanel = (PolylineEntity)sketch.Entities[0];
        var backPanel = (PolylineEntity)sketch.Entities[1];
        var leftPanel = (PolylineEntity)sketch.Entities[2];
        Assert.That(backPanel.Bounds.MinX, Is.GreaterThan(frontPanel.Bounds.MinX));
        Assert.That(leftPanel.Bounds.MinX, Is.GreaterThan(backPanel.Bounds.MinX));
    }

    [Test]
    public void GenerateSketch_ShouldKeepMarginsBetweenPanels()
    {
        var generator = new BoxGenerator();

        Sketch sketch = generator.GenerateSketch(new BoxGeneratorOptions());

        for (int index = 1; index < sketch.Entities.Count; index++)
        {
            var previous = (PolylineEntity)sketch.Entities[index - 1];
            var current = (PolylineEntity)sketch.Entities[index];

            Assert.That(current.Bounds.MinX - previous.Bounds.MaxX, Is.GreaterThan(0.0));
        }
    }

    [Test]
    public void GenerateSketch_ShouldPutPanelContoursOnCutLayer()
    {
        var generator = new BoxGenerator();

        Sketch sketch = generator.GenerateSketch(new BoxGeneratorOptions(boxType: BoxGeneratorType.WithLid));

        Assert.That(sketch.Entities.Select(entity => entity.LayerName), Is.All.EqualTo(DefaultLayers.Cut.Name));
    }

    [Test]
    public void GenerateSketch_OpenBox_ShouldCreateFivePanels()
    {
        var generator = new BoxGenerator();

        Sketch sketch = generator.GenerateSketch(new BoxGeneratorOptions(boxType: BoxGeneratorType.Open));

        Assert.That(sketch.Entities, Has.Count.EqualTo(5));
    }

    [Test]
    public void GenerateSketch_WhenWidthChanges_ShouldRebuildGeometry()
    {
        var generator = new BoxGenerator();
        var options = new BoxGeneratorOptions(width: Length.FromMillimeters(120.0));

        Sketch sketch = generator.GenerateSketch(options);

        var frontPanel = (PolylineEntity)sketch.Entities[0];
        var bottomPanel = (PolylineEntity)sketch.Entities[4];
        AssertPanelBounds(frontPanel.Bounds, expectedWidth: 126.0, expectedHeight: 56.0);
        AssertPanelBounds(bottomPanel.Bounds, expectedWidth: 126.0, expectedHeight: 86.0);
    }

    [Test]
    public void GenerateSketch_WhenMaterialThicknessChanges_ShouldRebuildFingerJoints()
    {
        var generator = new BoxGenerator();
        var options = new BoxGeneratorOptions(materialThickness: Length.FromMillimeters(5.0));

        Sketch sketch = generator.GenerateSketch(options);

        var frontPanel = (PolylineEntity)sketch.Entities[0];
        AssertPanelBounds(frontPanel.Bounds, expectedWidth: 110.0, expectedHeight: 60.0);
    }

    private static void AssertPanelBounds(BoundingBox bounds, double expectedWidth, double expectedHeight)
    {
        Assert.That(bounds.MaxX - bounds.MinX, Is.EqualTo(expectedWidth).Within(0.000001));
        Assert.That(bounds.MaxY - bounds.MinY, Is.EqualTo(expectedHeight).Within(0.000001));
    }
}
