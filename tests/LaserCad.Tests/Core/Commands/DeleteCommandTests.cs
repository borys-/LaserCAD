using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Commands;

public sealed class DeleteCommandTests
{
    [Test]
    public void Execute_ShouldDeleteEntity()
    {
        var removed = new LineEntity(new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(1.0, 1.0)));
        var kept = new LineEntity(new LineSegment2D(new Point2D(2.0, 2.0), new Point2D(3.0, 3.0)));
        var sketch = new Sketch(entities: new[] { removed, kept });
        var document = new CadDocument(sketches: new[] { sketch });
        var command = new DeleteCommand(sketch.Id, removed);

        var updated = command.Execute(document);

        Assert.That(updated.Sketches[0].Entities, Is.EqualTo(new[] { kept }));
    }
}
