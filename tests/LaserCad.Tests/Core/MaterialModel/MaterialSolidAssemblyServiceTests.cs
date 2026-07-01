using LaserCad.Core.Documents;
using LaserCad.Core.MaterialModel;
using LaserCad.Core.Preview3D;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.MaterialModel;

public sealed class MaterialSolidAssemblyServiceTests
{
    [Test]
    public void AttachToSurface_ShouldClampAnchorToTargetTopSurface()
    {
        var moving = CreateSolid("Nowa plyta", 0.0, 0.0);
        var target = CreateSolid("Bazowa plyta", 10.0, 20.0);
        var service = new MaterialSolidAssemblyService();

        var attachment = service.AttachToSurface(moving, target, new Point3D(99.0, 21.0, 0.0));

        Assert.That(attachment.TargetMaterialSolidId, Is.EqualTo(target.Id));
        Assert.That(attachment.AnchorPoint, Is.EqualTo(new Point3D(20.0, 21.0, 3.0)));
        Assert.That(attachment.Orientation.Position, Is.EqualTo(new Point3D(20.0, 21.0, 3.0)));
        Assert.That(attachment.Orientation.SurfaceNormal, Is.EqualTo(target.Orientation.SurfaceNormal));
    }

    [Test]
    public void CreateRightAngleJoint_ShouldRotateNormalAndOrientationByNinetyDegrees()
    {
        var moving = CreateSolid("Nowa plyta", 0.0, 0.0);
        var target = CreateSolid("Bazowa plyta", 10.0, 20.0);
        var service = new MaterialSolidAssemblyService();

        var orientation = service.CreateRightAngleJoint(moving, target, new Point3D(10.0, 20.0, 3.0));

        Assert.That(orientation.Position, Is.EqualTo(new Point3D(10.0, 20.0, 3.0)));
        Assert.That(orientation.RotationRadians, Is.EqualTo(Math.PI / 2.0));
        Assert.That(orientation.SurfaceNormal, Is.EqualTo(Vector3D.UnitX));
    }

    [Test]
    public void CreateRightAnglePreview_ShouldDescribeMountingRelation()
    {
        var moving = CreateSolid("Nowa plyta", 0.0, 0.0);
        var target = CreateSolid("Bazowa plyta", 10.0, 20.0);
        var service = new MaterialSolidAssemblyService();

        var preview = service.CreateRightAnglePreview(moving, target, new Point3D(10.0, 20.0, 3.0));

        Assert.That(preview.SourceMaterialSolidId, Is.EqualTo(moving.Id));
        Assert.That(preview.TargetMaterialSolidId, Is.EqualTo(target.Id));
        Assert.That(preview.AnchorPoint, Is.EqualTo(new Point3D(10.0, 20.0, 3.0)));
        Assert.That(preview.AngleRadians, Is.EqualTo(Math.PI / 2.0));
    }

    private static MaterialSolid CreateSolid(string name, double offsetX, double offsetY)
    {
        var solid = MaterialSolid.FromRectangle(
            name,
            new RectangleEntity(new Point2D(0.0, 0.0), 10.0, 5.0),
            DefaultMaterialProfiles.Plywood3Mm);

        return solid.WithOrientation(new MaterialSolidOrientation(
            new Point3D(offsetX, offsetY, 0.0),
            0.0,
            Vector3D.UnitZ));
    }
}
