using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Commands;

public sealed class RotateCommandTests
{
    [Test]
    public void Execute_ShouldRotateSelectedEntity()
    {
        var entity = new LineEntity(new LineSegment2D(new Point2D(2.0, 0.0), new Point2D(4.0, 0.0)));
        var sketch = new Sketch(entities: new[] { entity });
        var document = new CadDocument(sketches: new[] { sketch });
        var command = new RotateCommand(sketch.Id, entity.Id, Math.PI / 2.0);

        var updated = command.Execute(document);

        var rotated = (LineEntity)updated.Sketches[0].Entities[0];
        Assert.That(rotated.Segment.Start.X, Is.EqualTo(0.0).Within(GeometryTolerance.Default));
        Assert.That(rotated.Segment.Start.Y, Is.EqualTo(2.0).Within(GeometryTolerance.Default));
        Assert.That(rotated.Segment.End.X, Is.EqualTo(0.0).Within(GeometryTolerance.Default));
        Assert.That(rotated.Segment.End.Y, Is.EqualTo(4.0).Within(GeometryTolerance.Default));
    }
}
