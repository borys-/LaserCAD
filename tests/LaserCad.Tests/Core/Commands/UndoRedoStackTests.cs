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

    [Test]
    public void Redo_AfterUndo_ShouldApplyCommandAgain()
    {
        var original = CreateLine();
        var sketch = new Sketch(entities: new[] { original });
        var document = new CadDocument(sketches: new[] { sketch });
        var history = new UndoRedoStack(document);

        history.Execute(new MoveCommand(sketch.Id, original.Id, 5.0, 0.0));
        history.Undo();

        var redone = history.Redo();

        Assert.That(history.CanUndo, Is.True);
        Assert.That(history.CanRedo, Is.False);
        AssertLineStartX(redone, 6.0);
    }

    [Test]
    public void Execute_AfterUndo_ShouldClearRedoHistory()
    {
        var original = CreateLine();
        var sketch = new Sketch(entities: new[] { original });
        var document = new CadDocument(sketches: new[] { sketch });
        var history = new UndoRedoStack(document);

        history.Execute(new MoveCommand(sketch.Id, original.Id, 5.0, 0.0));
        history.Undo();
        history.Execute(new MoveCommand(sketch.Id, original.Id, 0.0, 2.0));

        Assert.That(history.CanRedo, Is.False);
        Assert.That(history.RedoCount, Is.EqualTo(0));
    }

    [Test]
    public void Execute_WhenHistoryLimitIsReached_ShouldDropOldestUndoCommand()
    {
        var original = CreateLine();
        var sketch = new Sketch(entities: new[] { original });
        var document = new CadDocument(sketches: new[] { sketch });
        var history = new UndoRedoStack(document, historyLimit: 1);

        history.Execute(new MoveCommand(sketch.Id, original.Id, 5.0, 0.0));
        history.Execute(new MoveCommand(sketch.Id, original.Id, 2.0, 0.0));

        var restored = history.Undo();

        Assert.That(history.CanUndo, Is.False);
        AssertLineStartX(restored, 6.0);
    }

    [Test]
    public void Undo_WithCommandGroup_ShouldUndoAllCommandsAsSingleHistoryItem()
    {
        var original = CreateLine();
        var sketch = new Sketch(entities: new[] { original });
        var document = new CadDocument(sketches: new[] { sketch });
        var history = new UndoRedoStack(document);
        var group = new CommandGroup(new ICommand[]
        {
            new MoveCommand(sketch.Id, original.Id, 5.0, 0.0),
            new MoveCommand(sketch.Id, original.Id, 2.0, 0.0),
        });

        history.Execute(group);
        var restored = history.Undo();

        Assert.That(history.UndoCount, Is.EqualTo(0));
        Assert.That(history.RedoCount, Is.EqualTo(1));
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
