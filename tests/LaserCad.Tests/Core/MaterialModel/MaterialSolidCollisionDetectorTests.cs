using LaserCad.Core.Documents;
using LaserCad.Core.MaterialModel;
using LaserCad.Core.Preview3D;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.MaterialModel;

public sealed class MaterialSolidCollisionDetectorTests
{
    [Test]
    public void FindCollisions_WithOverlappingSolids_ShouldReturnCollision()
    {
        var first = CreateSolid(0.0, 0.0);
        var second = CreateSolid(5.0, 2.0);
        var detector = new MaterialSolidCollisionDetector();

        var collisions = detector.FindCollisions(new[] { first, second });

        Assert.That(collisions, Has.Count.EqualTo(1));
        Assert.That(collisions[0].FirstMaterialSolidId, Is.EqualTo(first.Id));
        Assert.That(collisions[0].SecondMaterialSolidId, Is.EqualTo(second.Id));
    }

    [Test]
    public void FindCollisions_WithSeparatedSolids_ShouldReturnNoCollision()
    {
        var first = CreateSolid(0.0, 0.0);
        var second = CreateSolid(20.0, 0.0);
        var detector = new MaterialSolidCollisionDetector();

        var collisions = detector.FindCollisions(new[] { first, second });

        Assert.That(collisions, Is.Empty);
    }

    private static MaterialSolid CreateSolid(double offsetX, double offsetY)
    {
        var solid = MaterialSolid.FromRectangle(
            "Plyta",
            new RectangleEntity(new Point2D(0.0, 0.0), 10.0, 5.0),
            DefaultMaterialProfiles.Plywood3Mm);

        return solid.WithOrientation(new MaterialSolidOrientation(
            new Point3D(offsetX, offsetY, 0.0),
            0.0,
            Vector3D.UnitZ));
    }
}
