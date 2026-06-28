using LaserCad.Core.Documents;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Documents;

public sealed class DefaultMaterialProfilesTests
{
    [Test]
    public void All_ShouldContainStandardProfiles()
    {
        Assert.That(DefaultMaterialProfiles.All, Has.Count.EqualTo(4));
        Assert.That(
            DefaultMaterialProfiles.All,
            Is.EqualTo(new[]
            {
                DefaultMaterialProfiles.Plywood3Mm,
                DefaultMaterialProfiles.Plywood4Mm,
                DefaultMaterialProfiles.Mdf,
                DefaultMaterialProfiles.Acrylic
            }));
    }

    [Test]
    public void Profiles_ShouldDefineNamesAndThicknesses()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DefaultMaterialProfiles.Plywood3Mm.Name, Is.EqualTo("Sklejka 3 mm"));
            Assert.That(DefaultMaterialProfiles.Plywood3Mm.Thickness, Is.EqualTo(Length.FromMillimeters(3.0)));

            Assert.That(DefaultMaterialProfiles.Plywood4Mm.Name, Is.EqualTo("Sklejka 4 mm"));
            Assert.That(DefaultMaterialProfiles.Plywood4Mm.Thickness, Is.EqualTo(Length.FromMillimeters(4.0)));

            Assert.That(DefaultMaterialProfiles.Mdf.Name, Is.EqualTo("MDF 3 mm"));
            Assert.That(DefaultMaterialProfiles.Mdf.Thickness, Is.EqualTo(Length.FromMillimeters(3.0)));

            Assert.That(DefaultMaterialProfiles.Acrylic.Name, Is.EqualTo("Akryl 3 mm"));
            Assert.That(DefaultMaterialProfiles.Acrylic.Thickness, Is.EqualTo(Length.FromMillimeters(3.0)));
        });
    }

    [Test]
    public void Profiles_ShouldDefineProductionDefaults()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DefaultMaterialProfiles.Plywood3Mm.DefaultKerf, Is.EqualTo(Length.FromMillimeters(0.15)));
            Assert.That(DefaultMaterialProfiles.Plywood3Mm.DefaultClearance, Is.EqualTo(Length.FromMillimeters(0.1)));
            Assert.That(DefaultMaterialProfiles.Plywood3Mm.MinimumFingerWidth, Is.EqualTo(Length.FromMillimeters(3.0)));

            Assert.That(DefaultMaterialProfiles.Plywood4Mm.DefaultKerf, Is.EqualTo(Length.FromMillimeters(0.16)));
            Assert.That(DefaultMaterialProfiles.Plywood4Mm.DefaultClearance, Is.EqualTo(Length.FromMillimeters(0.1)));
            Assert.That(DefaultMaterialProfiles.Plywood4Mm.MinimumFingerWidth, Is.EqualTo(Length.FromMillimeters(4.0)));

            Assert.That(DefaultMaterialProfiles.Mdf.DefaultKerf, Is.EqualTo(Length.FromMillimeters(0.18)));
            Assert.That(DefaultMaterialProfiles.Mdf.DefaultClearance, Is.EqualTo(Length.FromMillimeters(0.1)));
            Assert.That(DefaultMaterialProfiles.Mdf.MinimumFingerWidth, Is.EqualTo(Length.FromMillimeters(3.0)));

            Assert.That(DefaultMaterialProfiles.Acrylic.DefaultKerf, Is.EqualTo(Length.FromMillimeters(0.14)));
            Assert.That(DefaultMaterialProfiles.Acrylic.DefaultClearance, Is.EqualTo(Length.FromMillimeters(0.08)));
            Assert.That(DefaultMaterialProfiles.Acrylic.MinimumFingerWidth, Is.EqualTo(Length.FromMillimeters(3.0)));
        });
    }
}
