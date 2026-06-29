using LaserCad.Core.Documents;
using LaserCad.Core.Parameters;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Documents;

public sealed class SketchTests
{
    [Test]
    public void Constructor_ShouldCreateSketch()
    {
        var sketch = new Sketch();

        Assert.That(sketch.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(sketch.Name, Is.EqualTo("Sketch"));
    }

    [Test]
    public void Constructor_WithIdAndName_ShouldStoreValues()
    {
        var id = Guid.NewGuid();

        var sketch = new Sketch(id, "Front");

        Assert.That(sketch.Id, Is.EqualTo(id));
        Assert.That(sketch.Name, Is.EqualTo("Front"));
    }

    [Test]
    public void Constructor_WithEmptyName_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = new Sketch(name: ""));
    }

    [Test]
    public void Constructor_ShouldCreateEmptyEntityCollection()
    {
        var sketch = new Sketch();

        Assert.That(sketch.Entities, Is.Empty);
    }

    [Test]
    public void AddEntity_ShouldReturnSketchWithAddedEntity()
    {
        var sketch = new Sketch();
        var entity = new TestEntity();

        var updatedSketch = sketch.AddEntity(entity);

        Assert.That(updatedSketch.Entities, Is.EqualTo(new[] { entity }));
        Assert.That(sketch.Entities, Is.Empty);
    }

    [Test]
    public void RemoveEntity_ShouldReturnSketchWithoutEntity()
    {
        var removedEntity = new TestEntity();
        var keptEntity = new TestEntity();
        var sketch = new Sketch(entities: new[] { removedEntity, keptEntity });

        var updatedSketch = sketch.RemoveEntity(removedEntity.Id);

        Assert.That(updatedSketch.Entities, Is.EqualTo(new[] { keptEntity }));
        Assert.That(sketch.Entities, Is.EqualTo(new[] { removedEntity, keptEntity }));
    }

    [Test]
    public void CopyEntity_ShouldAppendCopyWithNewId()
    {
        var copiedId = Guid.NewGuid();
        var entity = new LineEntity(new LineSegment2D(new Point2D(1.0, 2.0), new Point2D(3.0, 4.0)));
        var sketch = new Sketch(entities: new[] { entity });

        var updatedSketch = sketch.CopyEntity(entity.Id, copiedId);

        Assert.That(updatedSketch.Entities, Has.Count.EqualTo(2));
        var copy = (LineEntity)updatedSketch.Entities[1];
        Assert.That(copy.Id, Is.EqualTo(copiedId));
        Assert.That(copy.LayerName, Is.EqualTo(entity.LayerName));
        Assert.That(copy.Segment, Is.EqualTo(entity.Segment));
    }

    [Test]
    public void MoveEntity_ShouldTransformOnlySelectedEntity()
    {
        var movedEntity = new LineEntity(new LineSegment2D(new Point2D(1.0, 2.0), new Point2D(3.0, 4.0)));
        var keptEntity = new LineEntity(new LineSegment2D(new Point2D(10.0, 20.0), new Point2D(30.0, 40.0)));
        var sketch = new Sketch(entities: new[] { movedEntity, keptEntity });

        var updatedSketch = sketch.MoveEntity(movedEntity.Id, 5.0, 6.0);

        var moved = (LineEntity)updatedSketch.Entities[0];
        Assert.That(moved.Id, Is.EqualTo(movedEntity.Id));
        Assert.That(moved.Segment.Start, Is.EqualTo(new Point2D(6.0, 8.0)));
        Assert.That(moved.Segment.End, Is.EqualTo(new Point2D(8.0, 10.0)));
        Assert.That(updatedSketch.Entities[1], Is.SameAs(keptEntity));
    }

    [Test]
    public void RotateScaleAndMirrorEntity_ShouldUseGeometryTransforms()
    {
        var line = new LineEntity(new LineSegment2D(new Point2D(2.0, 0.0), new Point2D(4.0, 0.0)));
        var sketch = new Sketch(entities: new[] { line });

        var rotated = (LineEntity)sketch.RotateEntity(line.Id, Math.PI / 2.0).Entities[0];
        var scaled = (LineEntity)sketch.ScaleEntity(line.Id, 2.0, 3.0).Entities[0];
        var mirrored = (LineEntity)sketch.MirrorEntity(line.Id, SketchMirrorAxis.Y).Entities[0];

        Assert.That(rotated.Segment.Start.X, Is.EqualTo(0.0).Within(GeometryTolerance.Default));
        Assert.That(rotated.Segment.Start.Y, Is.EqualTo(2.0).Within(GeometryTolerance.Default));
        Assert.That(scaled.Segment.End, Is.EqualTo(new Point2D(8.0, 0.0)));
        Assert.That(mirrored.Segment.Start, Is.EqualTo(new Point2D(-2.0, 0.0)));
        Assert.That(mirrored.Segment.End, Is.EqualTo(new Point2D(-4.0, 0.0)));
    }

    [Test]
    public void RebuildFromParameters_ShouldUpdateBoundSketchEntities()
    {
        var rectangle = new RectangleEntity(new Point2D(0.0, 0.0), 10.0, 5.0)
            .BindDimension(new EntityDimensionBinding(EntityDimensionKind.Width, new ParameterId("Width")))
            .BindDimension(new EntityDimensionBinding(EntityDimensionKind.Height, new ParameterId("Height")));
        var circle = new CircleEntity(new Circle2D(new Point2D(50.0, 50.0), 4.0))
            .BindDimension(new EntityDimensionBinding(EntityDimensionKind.Diameter, new ParameterId("Diameter")));
        var sketch = new Sketch(entities: new Entity[] { rectangle, circle });
        var parameters = new ParameterSet(new[]
        {
            new Parameter(new ParameterId("Width"), "Width", ParameterType.Length, Length.FromMillimeters(20.0)),
            new Parameter(new ParameterId("Height"), "Height", ParameterType.Length, Length.FromMillimeters(12.0)),
            new Parameter(new ParameterId("Diameter"), "Diameter", ParameterType.Length, Length.FromMillimeters(30.0)),
        });

        var rebuilt = sketch.RebuildFromParameters(parameters);

        var rebuiltRectangle = (RectangleEntity)rebuilt.Entities[0];
        var rebuiltCircle = (CircleEntity)rebuilt.Entities[1];
        Assert.That(rebuiltRectangle.Bounds, Is.EqualTo(new BoundingBox(0.0, 0.0, 20.0, 12.0)));
        Assert.That(rebuiltCircle.Circle.Radius, Is.EqualTo(15.0));
        Assert.That(rebuiltRectangle.Id, Is.EqualTo(rectangle.Id));
        Assert.That(rebuiltCircle.Id, Is.EqualTo(circle.Id));
        Assert.That(sketch.Entities[0], Is.SameAs(rectangle));
    }

    private sealed class TestEntity : Entity
    {
        public override BoundingBox Bounds => new(0.0, 0.0, 1.0, 1.0);

        public override ISketchEntity Transform(Matrix3x3 transform)
        {
            return new TestEntity();
        }

        public override Entity Copy(Guid? id = null)
        {
            return new TestEntity(id);
        }

        public TestEntity(Guid? id = null)
            : base(id)
        {
        }
    }
}
