using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Commands;

public sealed class MirrorCommandTests
{
    [Test]
    public void Execute_ShouldMirrorSelectedEntity()
    {
        var entity = new LineEntity(new LineSegment2D(new Point2D(2.0, 3.0), new Point2D(4.0, 5.0)));
        var sketch = new Sketch(entities: new[] { entity });
        var document = new CadDocument(sketches: new[] { sketch });
        var command = new MirrorCommand(sketch.Id, entity.Id, SketchMirrorAxis.Y);

        var updated = command.Execute(document);

        var mirrored = (LineEntity)updated.Sketches[0].Entities[0];
        Assert.That(mirrored.Segment.Start, Is.EqualTo(new Point2D(-2.0, 3.0)));
        Assert.That(mirrored.Segment.End, Is.EqualTo(new Point2D(-4.0, 5.0)));
    }
}
