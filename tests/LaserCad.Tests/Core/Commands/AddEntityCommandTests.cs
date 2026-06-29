using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Commands;

public sealed class AddEntityCommandTests
{
    [Test]
    public void Execute_ShouldAddEntity()
    {
        var entity = new LineEntity(new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(1.0, 1.0)));
        var sketch = new Sketch();
        var document = new CadDocument(sketches: new[] { sketch });
        var command = new AddEntityCommand(sketch.Id, entity);

        var updated = command.Execute(document);

        Assert.That(updated.Sketches[0].Entities, Is.EqualTo(new[] { entity }));
        Assert.That(document.Sketches[0].Entities, Is.Empty);
    }
}
