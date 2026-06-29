using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Commands;

public sealed class MoveCommandTests
{
    [Test]
    public void Execute_ShouldMoveSelectedEntity()
    {
        var entity = new LineEntity(new LineSegment2D(new Point2D(1.0, 2.0), new Point2D(3.0, 4.0)));
        var sketch = new Sketch(entities: new[] { entity });
        var document = new CadDocument(sketches: new[] { sketch });
        var command = new MoveCommand(sketch.Id, entity.Id, 10.0, 20.0);

        var updated = command.Execute(document);

        var moved = (LineEntity)updated.Sketches[0].Entities[0];
        Assert.That(moved.Segment.Start, Is.EqualTo(new Point2D(11.0, 22.0)));
        Assert.That(moved.Segment.End, Is.EqualTo(new Point2D(13.0, 24.0)));
        Assert.That(((LineEntity)document.Sketches[0].Entities[0]).Segment.Start, Is.EqualTo(new Point2D(1.0, 2.0)));
    }
}
