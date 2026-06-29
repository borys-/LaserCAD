using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.Documents;

public sealed class EntityTests
{
    [Test]
    public void Entity_ShouldBeAbstract()
    {
        Assert.That(typeof(Entity).IsAbstract, Is.True);
    }

    [Test]
    public void Constructor_ShouldCreateEntityId()
    {
        var entity = new TestEntity();

        Assert.That(entity.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Constructor_WithId_ShouldStoreId()
    {
        var id = Guid.NewGuid();

        var entity = new TestEntity(id);

        Assert.That(entity.Id, Is.EqualTo(id));
    }

    [Test]
    public void Constructor_WithLayerName_ShouldStoreLayerName()
    {
        var entity = new TestEntity(layerName: "Engrave");

        Assert.That(entity.LayerName, Is.EqualTo("Engrave"));
    }

    [Test]
    public void Constructor_WithEmptyLayerName_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = new TestEntity(layerName: ""));
    }

    private sealed class TestEntity : Entity
    {
        public TestEntity(Guid? id = null, string layerName = "Cut")
            : base(id, layerName)
        {
        }

        public override BoundingBox Bounds => new(0.0, 0.0, 1.0, 1.0);

        public override ISketchEntity Transform(Matrix3x3 transform)
        {
            return new TestEntity(Id, LayerName);
        }
    }
}
