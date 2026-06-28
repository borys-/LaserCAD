using LaserCad.Core.Documents;

namespace LaserCad.Tests.Core.Documents;

public sealed class MaterialProfileTests
{
    [Test]
    public void Constructor_ShouldCreateMaterialProfile()
    {
        var profile = new MaterialProfile("Plywood 3 mm");

        Assert.That(profile, Is.Not.Null);
    }
}
