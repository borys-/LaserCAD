using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Commands;

public sealed class CommandExecuteTests
{
    [Test]
    public void Execute_ShouldApplyEveryEditingCommand()
    {
        var original = new LineEntity(new LineSegment2D(new Point2D(1.0, 1.0), new Point2D(2.0, 1.0)));
        var added = new LineEntity(new LineSegment2D(new Point2D(10.0, 10.0), new Point2D(20.0, 20.0)));
        var sketch = new Sketch(entities: new[] { original });
        var document = new CadDocument(sketches: new[] { sketch });

        Assert.That(EntityAfter(new MoveCommand(sketch.Id, original.Id, 2.0, 3.0).Execute(document)).Bounds, Is.EqualTo(new BoundingBox(3.0, 4.0, 4.0, 4.0)));
        Assert.That(EntityAfter(new RotateCommand(sketch.Id, original.Id, Math.PI / 2.0).Execute(document)).Bounds.MinY, Is.EqualTo(1.0).Within(GeometryTolerance.Default));
        Assert.That(EntityAfter(new ScaleCommand(sketch.Id, original.Id, 2.0, 3.0).Execute(document)).Bounds, Is.EqualTo(new BoundingBox(2.0, 3.0, 4.0, 3.0)));
        Assert.That(EntityAfter(new MirrorCommand(sketch.Id, original.Id, SketchMirrorAxis.Y).Execute(document)).Bounds, Is.EqualTo(new BoundingBox(-2.0, 1.0, -1.0, 1.0)));
        Assert.That(new DeleteCommand(sketch.Id, original).Execute(document).Sketches[0].Entities, Is.Empty);
        Assert.That(new AddEntityCommand(sketch.Id, added).Execute(document).Sketches[0].Entities, Has.Count.EqualTo(2));
    }

    private static Entity EntityAfter(CadDocument document)
    {
        return document.Sketches[0].Entities[0];
    }
}
