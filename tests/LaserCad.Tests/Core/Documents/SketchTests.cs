using LaserCad.Core.Documents;

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

    private sealed class TestEntity : Entity
    {
    }
}
