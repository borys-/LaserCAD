using LaserCad.Core.Documents;
using LaserCad.Core.MaterialModel;
using LaserCad.Core.Preview3D;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.MaterialModel;

public sealed class MaterialSolidSnapServiceTests
{
    [Test]
    public void FindNearest_NearCorner_ShouldReturnCornerSnap()
    {
        var solid = CreateSolid();
        var service = new MaterialSolidSnapService();

        var snap = service.FindNearest(new[] { solid }, new Point3D(0.2, 0.1, 0.0), 1.0);

        Assert.That(snap, Is.Not.Null);
        Assert.That(snap!.MaterialSolidId, Is.EqualTo(solid.Id));
        Assert.That(snap.Kind, Is.EqualTo(MaterialSolidSnapKind.Corner));
        Assert.That(snap.Position, Is.EqualTo(new Point3D(0.0, 0.0, 0.0)));
    }

    [Test]
    public void FindNearest_NearEdge_ShouldReturnEdgeSnap()
    {
        var solid = CreateSolid();
        var service = new MaterialSolidSnapService();

        var snap = service.FindNearest(new[] { solid }, new Point3D(5.0, -0.2, 0.0), 1.0);

        Assert.That(snap, Is.Not.Null);
        Assert.That(snap!.Kind, Is.EqualTo(MaterialSolidSnapKind.Edge));
        Assert.That(snap.Position, Is.EqualTo(new Point3D(5.0, 0.0, 0.0)));
    }

    [Test]
    public void FindNearest_OutsideTolerance_ShouldReturnNull()
    {
        var solid = CreateSolid();
        var service = new MaterialSolidSnapService();

        var snap = service.FindNearest(new[] { solid }, new Point3D(20.0, 20.0, 0.0), 1.0);

        Assert.That(snap, Is.Null);
    }

    private static MaterialSolid CreateSolid()
    {
        return MaterialSolid.FromRectangle(
            "Plyta",
            new RectangleEntity(new Point2D(0.0, 0.0), 10.0, 5.0),
            DefaultMaterialProfiles.Plywood3Mm);
    }
}
