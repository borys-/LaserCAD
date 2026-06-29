using LaserCad.Core.Dimensions;
using LaserCad.Core.Documents;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Dimensions;

public sealed class DimensionApplyTests
{
    [Test]
    public void Apply_WithLineLength_ShouldResizeLine()
    {
        var line = new LineEntity(new LineSegment2D(new Point2D(1.0, 2.0), new Point2D(4.0, 6.0)));
        var sketch = new Sketch(entities: new[] { line });
        var dimension = new Dimension(line.Id, DimensionKind.Length, Length.FromMillimeters(10.0));

        var updatedSketch = dimension.Apply(sketch);

        var updatedLine = (LineEntity)updatedSketch.Entities[0];
        Assert.That(updatedLine.Id, Is.EqualTo(line.Id));
        Assert.That(updatedLine.Segment.Start, Is.EqualTo(line.Segment.Start));
        Assert.That(updatedLine.Segment.Length, Is.EqualTo(10.0).Within(GeometryTolerance.Default));
        Assert.That(sketch.Entities[0], Is.SameAs(line));
    }
}
