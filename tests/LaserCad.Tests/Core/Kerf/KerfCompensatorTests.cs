using LaserCad.Core.Documents;
using LaserCad.Core.Kerf;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Kerf;

public sealed class KerfCompensatorTests
{
    [Test]
    public void Classify_WithCounterclockwisePolygon_ShouldReturnOuter()
    {
        var polygon = CreateCounterclockwiseSquare();

        var classification = KerfCompensator.Classify(polygon);

        Assert.That(classification, Is.EqualTo(KerfContourClassification.Outer));
    }

    [Test]
    public void Classify_WithClockwisePolygon_ShouldReturnInner()
    {
        var polygon = new Polygon2D(CreateCounterclockwiseSquare().Vertices.Reverse());

        var classification = KerfCompensator.Classify(polygon);

        Assert.That(classification, Is.EqualTo(KerfContourClassification.Inner));
    }

    [Test]
    public void Compensate_WithPositiveOuterSquare_ShouldOffsetOutward()
    {
        var sketch = new Sketch(name: "Test")
            .AddEntity(new PolylineEntity(
                new Polyline2D(CreateCounterclockwiseSquare().Vertices, isClosed: true)));
        var options = new KerfCompensationOptions(Length.FromMillimeters(0.2));

        var compensated = KerfCompensator.Compensate(sketch, options);

        var entity = (PolylineEntity)compensated.Entities.Single();
        Assert.That(entity.Bounds.MinX, Is.EqualTo(-0.1).Within(1e-9));
        Assert.That(entity.Bounds.MinY, Is.EqualTo(-0.1).Within(1e-9));
        Assert.That(entity.Bounds.MaxX, Is.EqualTo(10.1).Within(1e-9));
        Assert.That(entity.Bounds.MaxY, Is.EqualTo(10.1).Within(1e-9));
    }

    [Test]
    public void Compensate_WithPositiveInnerSquare_ShouldOffsetInward()
    {
        var inner = new Polygon2D(CreateCounterclockwiseSquare().Vertices.Reverse());
        var sketch = new Sketch(name: "Test")
            .AddEntity(new PolylineEntity(new Polyline2D(inner.Vertices, isClosed: true)));
        var options = new KerfCompensationOptions(Length.FromMillimeters(0.2));

        var compensated = KerfCompensator.Compensate(sketch, options);

        var entity = (PolylineEntity)compensated.Entities.Single();
        Assert.That(entity.Bounds.MinX, Is.EqualTo(0.1).Within(1e-9));
        Assert.That(entity.Bounds.MinY, Is.EqualTo(0.1).Within(1e-9));
        Assert.That(entity.Bounds.MaxX, Is.EqualTo(9.9).Within(1e-9));
        Assert.That(entity.Bounds.MaxY, Is.EqualTo(9.9).Within(1e-9));
    }

    [Test]
    public void CreatePreview_ShouldExposeBeforeAndAfterSketches()
    {
        var sketch = new Sketch(name: "Nominal")
            .AddEntity(new RectangleEntity(new Point2D(0, 0), 10, 10));
        var options = new KerfCompensationOptions(Length.FromMillimeters(0.2));

        var preview = KerfCompensator.CreatePreview(sketch, options);

        Assert.That(preview.BeforeCompensation, Is.SameAs(sketch));
        Assert.That(preview.AfterCompensation.Name, Is.EqualTo("Nominal - kerf"));
        Assert.That(preview.AfterCompensation.Entities.Single(), Is.TypeOf<PolylineEntity>());
    }

    private static Polygon2D CreateCounterclockwiseSquare()
    {
        return new Polygon2D(new[]
        {
            new Point2D(0, 0),
            new Point2D(10, 0),
            new Point2D(10, 10),
            new Point2D(0, 10),
        });
    }
}
