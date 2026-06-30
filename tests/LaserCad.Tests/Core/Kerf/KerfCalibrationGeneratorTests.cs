using LaserCad.Core.Documents;
using LaserCad.Core.Kerf;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Kerf;

public sealed class KerfCalibrationGeneratorTests
{
    [Test]
    public void GenerateSketch_ShouldCreateFrameAndTestSlots()
    {
        var options = new KerfCalibrationOptions(
            slotCount: 3,
            slotWidth: Length.FromMillimeters(10.0),
            slotHeight: Length.FromMillimeters(4.0),
            spacing: Length.FromMillimeters(2.0),
            margin: Length.FromMillimeters(1.0));
        var generator = new KerfCalibrationGenerator();

        var sketch = generator.GenerateSketch(options);

        Assert.That(sketch.Name, Is.EqualTo("Probnik kerfu"));
        Assert.That(sketch.Entities.OfType<RectangleEntity>().ToArray(), Has.Length.EqualTo(4));

        var slots = sketch.Entities.OfType<RectangleEntity>().Skip(1).ToArray();
        Assert.That(slots.Select(slot => slot.Bounds.Width), Is.All.EqualTo(10.0).Within(1e-9));
        Assert.That(slots.Select(slot => slot.Bounds.Height), Is.All.EqualTo(4.0).Within(1e-9));
        Assert.That(slots.Select(slot => slot.Bounds.MinX), Is.EqualTo(new[] { 1.0, 13.0, 25.0 }).Within(1e-9));
        Assert.That(slots.Select(slot => slot.Bounds.MinY), Is.All.EqualTo(1.0).Within(1e-9));
    }

    [Test]
    public void GenerateSketch_ShouldAddEngravedKerfLabels()
    {
        var options = new KerfCalibrationOptions(
            baseKerf: Length.FromMillimeters(0.1),
            kerfStep: Length.FromMillimeters(0.05),
            slotCount: 3);
        var generator = new KerfCalibrationGenerator();

        var sketch = generator.GenerateSketch(options);

        var labels = sketch.Entities.OfType<TextEntity>().ToArray();
        Assert.That(labels.Select(label => label.Text), Is.EqualTo(new[] { "0.1 mm", "0.15 mm", "0.2 mm" }));
        Assert.That(labels.Select(label => label.LayerName), Is.All.EqualTo(DefaultLayers.Engrave.Name));
    }
}
