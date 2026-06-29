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

    [Test]
    public void Apply_WithRectangleWidth_ShouldResizeRectangleWidth()
    {
        var rectangle = new RectangleEntity(new Point2D(2.0, 3.0), 10.0, 5.0);
        var sketch = new Sketch(entities: new[] { rectangle });
        var dimension = new Dimension(rectangle.Id, DimensionKind.Width, Length.FromMillimeters(20.0));

        var updatedSketch = dimension.Apply(sketch);

        var updatedRectangle = (RectangleEntity)updatedSketch.Entities[0];
        Assert.That(updatedRectangle.Id, Is.EqualTo(rectangle.Id));
        Assert.That(updatedRectangle.Bounds, Is.EqualTo(new BoundingBox(2.0, 3.0, 22.0, 8.0)));
    }

    [Test]
    public void Apply_WithRectangleHeight_ShouldResizeRectangleHeight()
    {
        var rectangle = new RectangleEntity(new Point2D(2.0, 3.0), 10.0, 5.0);
        var sketch = new Sketch(entities: new[] { rectangle });
        var dimension = new Dimension(rectangle.Id, DimensionKind.Height, Length.FromMillimeters(12.0));

        var updatedSketch = dimension.Apply(sketch);

        var updatedRectangle = (RectangleEntity)updatedSketch.Entities[0];
        Assert.That(updatedRectangle.Id, Is.EqualTo(rectangle.Id));
        Assert.That(updatedRectangle.Bounds, Is.EqualTo(new BoundingBox(2.0, 3.0, 12.0, 15.0)));
    }

    [Test]
    public void Apply_WithCircleDiameter_ShouldResizeCircleDiameter()
    {
        var circle = new CircleEntity(new Circle2D(new Point2D(10.0, 20.0), 4.0));
        var sketch = new Sketch(entities: new[] { circle });
        var dimension = new Dimension(circle.Id, DimensionKind.Diameter, Length.FromMillimeters(30.0));

        var updatedSketch = dimension.Apply(sketch);

        var updatedCircle = (CircleEntity)updatedSketch.Entities[0];
        Assert.That(updatedCircle.Id, Is.EqualTo(circle.Id));
        Assert.That(updatedCircle.Circle.Center, Is.EqualTo(circle.Circle.Center));
        Assert.That(updatedCircle.Circle.Radius, Is.EqualTo(15.0));
    }

    [Test]
    public void Apply_WithCircleRadius_ShouldResizeCircleRadius()
    {
        var circle = new CircleEntity(new Circle2D(new Point2D(10.0, 20.0), 4.0));
        var sketch = new Sketch(entities: new[] { circle });
        var dimension = new Dimension(circle.Id, DimensionKind.Radius, Length.FromMillimeters(12.0));

        var updatedSketch = dimension.Apply(sketch);

        var updatedCircle = (CircleEntity)updatedSketch.Entities[0];
        Assert.That(updatedCircle.Id, Is.EqualTo(circle.Id));
        Assert.That(updatedCircle.Circle.Center, Is.EqualTo(circle.Circle.Center));
        Assert.That(updatedCircle.Circle.Radius, Is.EqualTo(12.0));
    }
}
