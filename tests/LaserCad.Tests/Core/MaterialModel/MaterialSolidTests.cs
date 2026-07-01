using LaserCad.Core.Documents;
using LaserCad.Core.MaterialModel;

namespace LaserCad.Tests.Core.MaterialModel;

public sealed class MaterialSolidTests
{
    [Test]
    public void Constructor_ShouldUseMaterialProfileThickness()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;

        var solid = new MaterialSolid("Plyta frontowa", material);

        Assert.That(solid.MaterialProfile, Is.SameAs(material));
        Assert.That(solid.Thickness.Millimeters, Is.EqualTo(3.0));
    }

    [Test]
    public void Constructor_WithEmptyId_ShouldThrow()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;

        Assert.Throws<ArgumentException>(() => new MaterialSolid(Guid.Empty, "Plyta frontowa", material));
    }

    [Test]
    public void Constructor_WithEmptyName_ShouldThrow()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;

        Assert.Throws<ArgumentException>(() => new MaterialSolid(" ", material));
    }

    [Test]
    public void Constructor_WithNullMaterialProfile_ShouldThrow()
    {
        Assert.Throws<ArgumentNullException>(() => new MaterialSolid("Plyta frontowa", null!));
    }
}
