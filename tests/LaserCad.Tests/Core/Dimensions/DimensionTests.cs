using LaserCad.Core.Dimensions;
using LaserCad.Core.Parameters;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Dimensions;

public sealed class DimensionTests
{
    [Test]
    public void Constructor_ShouldCreateDimension()
    {
        var entityId = Guid.NewGuid();
        var id = Guid.NewGuid();

        var dimension = new Dimension(
            entityId,
            DimensionKind.Length,
            Length.FromMillimeters(10.0),
            id,
            "Line length");

        Assert.That(dimension.Id, Is.EqualTo(id));
        Assert.That(dimension.EntityId, Is.EqualTo(entityId));
        Assert.That(dimension.Kind, Is.EqualTo(DimensionKind.Length));
        Assert.That(dimension.Value, Is.EqualTo(Length.FromMillimeters(10.0)));
        Assert.That(dimension.Name, Is.EqualTo("Line length"));
        Assert.That(dimension.ParameterId, Is.Null);
    }

    [Test]
    public void Constructor_WithInvalidData_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = new Dimension(
            Guid.Empty,
            DimensionKind.Length,
            Length.FromMillimeters(10.0)));

        Assert.Throws<ArgumentException>(() => _ = new Dimension(
            Guid.NewGuid(),
            DimensionKind.Length,
            Length.FromMillimeters(10.0),
            name: ""));

        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new Dimension(
            Guid.NewGuid(),
            DimensionKind.Length,
            Length.FromMillimeters(0.0)));
    }

    [Test]
    public void BindToParameter_ShouldReturnDimensionWithParameter()
    {
        var dimension = new Dimension(Guid.NewGuid(), DimensionKind.Width, Length.FromMillimeters(20.0));
        var parameterId = new ParameterId("Width");

        var bound = dimension.BindToParameter(parameterId);

        Assert.That(bound.Id, Is.EqualTo(dimension.Id));
        Assert.That(bound.EntityId, Is.EqualTo(dimension.EntityId));
        Assert.That(bound.Kind, Is.EqualTo(dimension.Kind));
        Assert.That(bound.Value, Is.EqualTo(dimension.Value));
        Assert.That(bound.ParameterId, Is.EqualTo(parameterId));
    }
}
