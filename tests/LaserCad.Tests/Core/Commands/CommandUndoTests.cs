using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Commands;

public sealed class CommandUndoTests
{
    [Test]
    public void Undo_ShouldRevertEveryEditingCommand()
    {
        var original = new LineEntity(new LineSegment2D(new Point2D(1.0, 1.0), new Point2D(2.0, 1.0)));
        var added = new LineEntity(new LineSegment2D(new Point2D(10.0, 10.0), new Point2D(20.0, 20.0)));
        var sketch = new Sketch(entities: new[] { original });
        var document = new CadDocument(sketches: new[] { sketch });

        AssertLineRestored(new MoveCommand(sketch.Id, original.Id, 2.0, 3.0), document, original);
        AssertLineRestored(new RotateCommand(sketch.Id, original.Id, Math.PI / 2.0), document, original);
        AssertLineRestored(new ScaleCommand(sketch.Id, original.Id, 2.0, 3.0), document, original);
        AssertLineRestored(new MirrorCommand(sketch.Id, original.Id, SketchMirrorAxis.Y), document, original);

        var deletedThenRestored = new DeleteCommand(sketch.Id, original).Undo(new DeleteCommand(sketch.Id, original).Execute(document));
        Assert.That(deletedThenRestored.Sketches[0].Entities.Select(entity => entity.Id), Is.EqualTo(new[] { original.Id }));

        var addedThenRemoved = new AddEntityCommand(sketch.Id, added).Undo(new AddEntityCommand(sketch.Id, added).Execute(document));
        Assert.That(addedThenRemoved.Sketches[0].Entities.Select(entity => entity.Id), Is.EqualTo(new[] { original.Id }));
    }

    private static void AssertLineRestored(ICommand command, CadDocument document, LineEntity original)
    {
        var restored = (LineEntity)command.Undo(command.Execute(document)).Sketches[0].Entities[0];

        Assert.That(restored.Segment.Start.X, Is.EqualTo(original.Segment.Start.X).Within(GeometryTolerance.Default));
        Assert.That(restored.Segment.Start.Y, Is.EqualTo(original.Segment.Start.Y).Within(GeometryTolerance.Default));
        Assert.That(restored.Segment.End.X, Is.EqualTo(original.Segment.End.X).Within(GeometryTolerance.Default));
        Assert.That(restored.Segment.End.Y, Is.EqualTo(original.Segment.End.Y).Within(GeometryTolerance.Default));
    }
}
