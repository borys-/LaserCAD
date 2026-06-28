using LaserCad.Core.Documents;

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

    private sealed class TestEntity : Entity
    {
        public TestEntity(Guid? id = null)
            : base(id)
        {
        }
    }
}
