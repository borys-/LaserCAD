using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Commands;

public sealed class ScaleCommandTests
{
    [Test]
    public void Execute_ShouldScaleSelectedEntity()
    {
        var entity = new LineEntity(new LineSegment2D(new Point2D(2.0, 3.0), new Point2D(4.0, 5.0)));
        var sketch = new Sketch(entities: new[] { entity });
        var document = new CadDocument(sketches: new[] { sketch });
        var command = new ScaleCommand(sketch.Id, entity.Id, 2.0, 3.0);

        var updated = command.Execute(document);

        var scaled = (LineEntity)updated.Sketches[0].Entities[0];
        Assert.That(scaled.Segment.Start, Is.EqualTo(new Point2D(4.0, 9.0)));
        Assert.That(scaled.Segment.End, Is.EqualTo(new Point2D(8.0, 15.0)));
    }
}
