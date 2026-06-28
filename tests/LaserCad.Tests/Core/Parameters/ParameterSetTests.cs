using LaserCad.Core.Parameters;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Parameters;

public sealed class ParameterSetTests
{
    [Test]
    public void Constructor_ShouldCreateEmptySet()
    {
        var parameterSet = new ParameterSet();

        Assert.That(parameterSet.Parameters, Is.Empty);
    }

    [Test]
    public void Constructor_WithParameters_ShouldStoreParameters()
    {
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 120.0);

        var parameterSet = new ParameterSet([parameter]);

        Assert.That(parameterSet.Parameters, Is.EqualTo(new[] { parameter }));
    }

    [Test]
    public void Constructor_WithNullParameters_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => _ = new ParameterSet(null!));
    }

    [Test]
    public void Constructor_WithNullParameter_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = new ParameterSet([null!]));
    }

    [Test]
    public void Add_ShouldReturnSetWithAddedParameter()
    {
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 120.0);
        var parameterSet = new ParameterSet();

        var updatedSet = parameterSet.Add(parameter);

        Assert.That(updatedSet.Parameters, Is.EqualTo(new[] { parameter }));
        Assert.That(parameterSet.Parameters, Is.Empty);
    }

    [Test]
    public void FindById_WithExistingId_ShouldReturnParameter()
    {
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 120.0);
        var parameterSet = new ParameterSet([parameter]);

        var result = parameterSet.FindById(new ParameterId("Width"));

        Assert.That(result, Is.SameAs(parameter));
    }

    [Test]
    public void FindById_WithMissingId_ShouldReturnNull()
    {
        var parameterSet = new ParameterSet();

        var result = parameterSet.FindById(new ParameterId("Width"));

        Assert.That(result, Is.Null);
    }

    [Test]
    public void FindByName_WithExistingName_ShouldReturnParameter()
    {
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 120.0);
        var parameterSet = new ParameterSet([parameter]);

        var result = parameterSet.FindByName("Width");

        Assert.That(result, Is.SameAs(parameter));
    }

    [Test]
    public void FindByName_WithMissingName_ShouldReturnNull()
    {
        var parameterSet = new ParameterSet();

        var result = parameterSet.FindByName("Width");

        Assert.That(result, Is.Null);
    }

    [Test]
    public void FindByName_WithEmptyName_ShouldThrow()
    {
        var parameterSet = new ParameterSet();

        Assert.Throws<ArgumentException>(() => _ = parameterSet.FindByName(""));
    }

    [Test]
    public void UpdateValue_WithExistingId_ShouldReturnSetWithUpdatedValue()
    {
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 120.0);
        var parameterSet = new ParameterSet([parameter]);

        var updatedSet = parameterSet.UpdateValue(new ParameterId("Width"), 150.0);

        Assert.That(updatedSet.FindById(new ParameterId("Width"))?.Value, Is.EqualTo(150.0));
        Assert.That(parameterSet.FindById(new ParameterId("Width"))?.Value, Is.EqualTo(120.0));
    }

    [Test]
    public void UpdateValue_WithMissingId_ShouldThrow()
    {
        var parameterSet = new ParameterSet();

        Assert.Throws<ArgumentException>(() => _ = parameterSet.UpdateValue(new ParameterId("Width"), 150.0));
    }

    [Test]
    public void UpdateValue_WithInvalidValue_ShouldThrow()
    {
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 120.0);
        var parameterSet = new ParameterSet([parameter]);

        Assert.Throws<ArgumentException>(() => _ = parameterSet.UpdateValue(new ParameterId("Width"), "150"));
    }

    [Test]
    public void UpdateValue_ShouldKeepParameterMetadata()
    {
        var parameter = new Parameter(
            new ParameterId("MaterialThickness"),
            "Material thickness",
            ParameterType.Length,
            Length.FromMillimeters(3.0),
            "mm",
            Length.FromMillimeters(2.0),
            Length.FromMillimeters(6.0));
        var parameterSet = new ParameterSet([parameter]);

        var updatedSet = parameterSet.UpdateValue(
            new ParameterId("MaterialThickness"),
            Length.FromMillimeters(4.0));
        var updatedParameter = updatedSet.FindById(new ParameterId("MaterialThickness"));

        Assert.That(updatedParameter?.Name, Is.EqualTo("Material thickness"));
        Assert.That(updatedParameter?.Type, Is.EqualTo(ParameterType.Length));
        Assert.That(updatedParameter?.DisplayUnit, Is.EqualTo("mm"));
        Assert.That(updatedParameter?.MinimumValue, Is.EqualTo(Length.FromMillimeters(2.0)));
        Assert.That(updatedParameter?.MaximumValue, Is.EqualTo(Length.FromMillimeters(6.0)));
        Assert.That(updatedParameter?.Value, Is.EqualTo(Length.FromMillimeters(4.0)));
    }

    [Test]
    public void UpdateValue_WithValueBelowMinimum_ShouldThrow()
    {
        var parameter = new Parameter(
            new ParameterId("Width"),
            "Width",
            ParameterType.Number,
            120.0,
            minimumValue: 10.0);
        var parameterSet = new ParameterSet([parameter]);

        Assert.Throws<ArgumentOutOfRangeException>(() => _ = parameterSet.UpdateValue(new ParameterId("Width"), 5.0));
    }

    [Test]
    public void UpdateValue_WithValueAboveMaximum_ShouldThrow()
    {
        var parameter = new Parameter(
            new ParameterId("Width"),
            "Width",
            ParameterType.Number,
            120.0,
            maximumValue: 200.0);
        var parameterSet = new ParameterSet([parameter]);

        Assert.Throws<ArgumentOutOfRangeException>(() => _ = parameterSet.UpdateValue(new ParameterId("Width"), 250.0));
    }
}
