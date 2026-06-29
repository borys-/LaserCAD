using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Commands;

public sealed class UndoRedoStackTests
{
    [Test]
    public void Undo_AfterExecutedCommand_ShouldRestorePreviousDocument()
    {
        var original = CreateLine();
        var sketch = new Sketch(entities: new[] { original });
        var document = new CadDocument(sketches: new[] { sketch });
        var history = new UndoRedoStack(document);

        history.Execute(new MoveCommand(sketch.Id, original.Id, 5.0, 0.0));

        var restored = history.Undo();

        Assert.That(history.CanUndo, Is.False);
        Assert.That(history.CanRedo, Is.True);
        AssertLineStartX(restored, original.Segment.Start.X);
    }

    private static LineEntity CreateLine()
    {
        return new LineEntity(new LineSegment2D(new Point2D(1.0, 1.0), new Point2D(3.0, 1.0)));
    }

    private static void AssertLineStartX(CadDocument document, double expected)
    {
        var line = (LineEntity)document.Sketches[0].Entities[0];
        Assert.That(line.Segment.Start.X, Is.EqualTo(expected).Within(GeometryTolerance.Default));
    }
}
