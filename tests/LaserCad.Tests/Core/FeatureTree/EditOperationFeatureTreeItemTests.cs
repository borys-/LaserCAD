using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Core.FeatureTree;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.FeatureTree;

public sealed class EditOperationFeatureTreeItemTests
{
    [Test]
    public void Constructor_ShouldStoreCommandAndKind()
    {
        var document = CreateDocument(out var sketch, out var line);
        var command = new MoveCommand(sketch.Id, line.Id, 5.0, 0.0);

        var item = new EditOperationFeatureTreeItem(command, name: "Move");

        Assert.That(item.Command, Is.SameAs(command));
        Assert.That(item.Kind, Is.EqualTo(FeatureTreeItemKind.EditOperation));
        Assert.That(document.Sketches, Has.Count.EqualTo(1));
    }

    [Test]
    public void Apply_ShouldExecuteCommand()
    {
        var document = CreateDocument(out var sketch, out var line);
        var command = new MoveCommand(sketch.Id, line.Id, 5.0, 0.0);
        var item = new EditOperationFeatureTreeItem(command, name: "Move");

        var rebuilt = item.Apply(document);

        var moved = (LineEntity)rebuilt.Sketches[0].Entities[0];
        Assert.That(moved.Segment.Start.X, Is.EqualTo(6.0).Within(GeometryTolerance.Default));
    }

    [Test]
    public void WithEnabled_ShouldPreserveCommand()
    {
        CreateDocument(out var sketch, out var line);
        var command = new MoveCommand(sketch.Id, line.Id, 5.0, 0.0);
        var item = new EditOperationFeatureTreeItem(command, name: "Move");

        var disabled = (EditOperationFeatureTreeItem)item.WithEnabled(false);

        Assert.That(disabled.IsEnabled, Is.False);
        Assert.That(disabled.Command, Is.SameAs(command));
    }

    private static CadDocument CreateDocument(out Sketch sketch, out LineEntity line)
    {
        line = new LineEntity(new LineSegment2D(new Point2D(1.0, 1.0), new Point2D(3.0, 1.0)));
        sketch = new Sketch(entities: new[] { line });
        return new CadDocument(sketches: new[] { sketch });
    }
}
