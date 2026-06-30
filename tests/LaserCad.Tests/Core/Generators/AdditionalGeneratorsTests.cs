using LaserCad.Core.Documents;
using LaserCad.Core.Generators;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Generators;

public sealed class AdditionalGeneratorsTests
{
    public static IEnumerable<TestCaseData> Generators()
    {
        yield return new TestCaseData(new TrayGenerator()).SetName("TrayGenerator_ShouldCreateSketch");
        yield return new TestCaseData(new OrganizerGenerator()).SetName("OrganizerGenerator_ShouldCreateSketch");
        yield return new TestCaseData(new DrawerGenerator()).SetName("DrawerGenerator_ShouldCreateSketch");
        yield return new TestCaseData(new DividerGenerator()).SetName("DividerGenerator_ShouldCreateSketch");
        yield return new TestCaseData(new PegboardGenerator()).SetName("PegboardGenerator_ShouldCreateSketch");
        yield return new TestCaseData(new FrameGenerator()).SetName("FrameGenerator_ShouldCreateSketch");
        yield return new TestCaseData(new StandGenerator()).SetName("StandGenerator_ShouldCreateSketch");
    }

    [TestCaseSource(nameof(Generators))]
    public void GenerateSketch_ShouldReturnNamedSketchWithEntities(ISketchGenerator generator)
    {
        Sketch sketch = generator.GenerateSketch();

        Assert.That(sketch.Name, Is.EqualTo(generator.Name));
        Assert.That(sketch.Entities, Is.Not.Empty);
        Assert.That(sketch.Entities.Select(entity => entity.LayerName), Does.Contain(DefaultLayers.Cut.Name));
    }

    [Test]
    public void TrayGenerator_ShouldCreateCutOutlineAndScoreInset()
    {
        Sketch sketch = new TrayGenerator().GenerateSketch();

        Assert.That(sketch.Entities.OfType<RectangleEntity>().Count(), Is.EqualTo(2));
        Assert.That(sketch.Entities[0].LayerName, Is.EqualTo(DefaultLayers.Cut.Name));
        Assert.That(sketch.Entities[1].LayerName, Is.EqualTo(DefaultLayers.Score.Name));
    }

    [Test]
    public void OrganizerGenerator_ShouldCreateDividerGrid()
    {
        Sketch sketch = new OrganizerGenerator(columns: 4, rows: 3).GenerateSketch();

        Assert.That(sketch.Entities.OfType<RectangleEntity>().Count(), Is.EqualTo(1));
        Assert.That(sketch.Entities.OfType<LineEntity>().Count(), Is.EqualTo(5));
    }

    [Test]
    public void DrawerGenerator_ShouldCreateHandleHole()
    {
        Sketch sketch = new DrawerGenerator().GenerateSketch();

        Assert.That(sketch.Entities.OfType<RectangleEntity>().Count(), Is.EqualTo(1));
        Assert.That(sketch.Entities.OfType<CircleEntity>().Count(), Is.EqualTo(1));
        Assert.That(sketch.Entities.OfType<LineEntity>().Count(), Is.EqualTo(1));
    }

    [Test]
    public void DividerGenerator_ShouldCreateRequestedDividerCountWithSlots()
    {
        Sketch sketch = new DividerGenerator(count: 4).GenerateSketch();

        Assert.That(sketch.Entities.OfType<RectangleEntity>().Count(), Is.EqualTo(8));
    }

    [Test]
    public void PegboardGenerator_ShouldCreateHoleGrid()
    {
        var options = new RectangularGeneratorOptions(
            width: Length.FromMillimeters(60.0),
            depth: Length.FromMillimeters(40.0));
        Sketch sketch = new PegboardGenerator(options, holeSpacing: Length.FromMillimeters(20.0)).GenerateSketch();

        Assert.That(sketch.Entities.OfType<RectangleEntity>().Count(), Is.EqualTo(1));
        Assert.That(sketch.Entities.OfType<CircleEntity>().Count(), Is.EqualTo(2));
    }

    [Test]
    public void FrameGenerator_ShouldCreateOuterAndInnerCuts()
    {
        Sketch sketch = new FrameGenerator().GenerateSketch();

        Assert.That(sketch.Entities.OfType<RectangleEntity>().Count(), Is.EqualTo(2));
        Assert.That(sketch.Entities.Select(entity => entity.LayerName), Is.All.EqualTo(DefaultLayers.Cut.Name));
    }

    [Test]
    public void StandGenerator_ShouldCreateTwoPartsAndTwoSlots()
    {
        Sketch sketch = new StandGenerator().GenerateSketch();

        Assert.That(sketch.Entities.OfType<RectangleEntity>().Count(), Is.EqualTo(4));
        Assert.That(sketch.Entities.Select(entity => entity.LayerName), Is.All.EqualTo(DefaultLayers.Cut.Name));
    }
}
