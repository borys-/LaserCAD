using LaserCad.Core.Documents;
using LaserCad.Core.MaterialModel;
using LaserCad.Core.Preview3D;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.MaterialModel;

public sealed class MaterialSolidTests
{
    [Test]
    public void Constructor_ShouldUseMaterialProfileThickness()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;
        var mesh = CreateMesh();

        var solid = new MaterialSolid("Plyta frontowa", material, mesh);

        Assert.That(solid.MaterialProfile, Is.SameAs(material));
        Assert.That(solid.Thickness.Millimeters, Is.EqualTo(3.0));
        Assert.That(solid.Mesh, Is.SameAs(mesh));
    }

    [Test]
    public void Constructor_WithEmptyId_ShouldThrow()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;
        var mesh = CreateMesh();

        Assert.Throws<ArgumentException>(() => new MaterialSolid(Guid.Empty, "Plyta frontowa", material, mesh));
    }

    [Test]
    public void Constructor_WithEmptyName_ShouldThrow()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;
        var mesh = CreateMesh();

        Assert.Throws<ArgumentException>(() => new MaterialSolid(" ", material, mesh));
    }

    [Test]
    public void Constructor_WithNullMaterialProfile_ShouldThrow()
    {
        var mesh = CreateMesh();

        Assert.Throws<ArgumentNullException>(() => new MaterialSolid("Plyta frontowa", null!, mesh));
    }

    [Test]
    public void Constructor_WithNullMesh_ShouldThrow()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;

        Assert.Throws<ArgumentNullException>(() => new MaterialSolid("Plyta frontowa", material, null!));
    }

    [Test]
    public void Constructor_WithMeshThicknessDifferentThanMaterial_ShouldThrow()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;
        var mesh = CreateMesh(4.0);

        Assert.Throws<ArgumentException>(() => new MaterialSolid("Plyta frontowa", material, mesh));
    }

    [Test]
    public void FromRectangle_ShouldCreateCuboidWithMaterialThickness()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;
        var rectangle = new RectangleEntity(new Point2D(0.0, 0.0), 20.0, 10.0);

        var solid = MaterialSolid.FromRectangle("Plyta frontowa", rectangle, material);

        Assert.That(solid.Name, Is.EqualTo("Plyta frontowa"));
        Assert.That(solid.Thickness.Millimeters, Is.EqualTo(3.0));
        Assert.That(solid.Mesh.Vertices, Has.Count.EqualTo(8));
        Assert.That(solid.Mesh.TriangleCount, Is.EqualTo(12));
        Assert.That(solid.Mesh.Vertices.Select(vertex => vertex.Z), Does.Contain(3.0));
    }

    [Test]
    public void FromRectangle_WithDocument_ShouldUseDocumentMaterialProfile()
    {
        var material = DefaultMaterialProfiles.Plywood4Mm;
        var document = new CadDocument().WithMaterialProfile(material);
        var rectangle = new RectangleEntity(new Point2D(0.0, 0.0), 20.0, 10.0);

        var solid = MaterialSolid.FromRectangle("Plyta frontowa", rectangle, document);

        Assert.That(solid.MaterialProfile, Is.SameAs(material));
        Assert.That(solid.Thickness.Millimeters, Is.EqualTo(4.0));
        Assert.That(solid.Mesh.Vertices.Select(vertex => vertex.Z), Does.Contain(4.0));
    }

    [Test]
    public void FromRectangle_WithDocumentWithoutMaterialProfile_ShouldThrow()
    {
        var document = new CadDocument();
        var rectangle = new RectangleEntity(new Point2D(0.0, 0.0), 20.0, 10.0);

        Assert.Throws<InvalidOperationException>(() =>
            MaterialSolid.FromRectangle("Plyta frontowa", rectangle, document));
    }

    [Test]
    public void Constructor_ShouldUseDefaultOrientation()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;
        var mesh = CreateMesh();

        var solid = new MaterialSolid("Plyta frontowa", material, mesh);

        Assert.That(solid.Orientation.Position, Is.EqualTo(new Point3D(0.0, 0.0, 0.0)));
        Assert.That(solid.Orientation.RotationRadians, Is.EqualTo(0.0));
        Assert.That(solid.Orientation.SurfaceNormal, Is.EqualTo(Vector3D.UnitZ));
    }

    [Test]
    public void Constructor_ShouldPreserveOrientation()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;
        var mesh = CreateMesh();
        var orientation = new MaterialSolidOrientation(
            new Point3D(10.0, 20.0, 30.0),
            Math.PI / 2.0,
            new Vector3D(0.0, 1.0, 0.0));

        var solid = new MaterialSolid("Plyta frontowa", material, mesh, orientation);

        Assert.That(solid.Orientation, Is.SameAs(orientation));
    }

    [Test]
    public void Orientation_WithZeroNormal_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() =>
            new MaterialSolidOrientation(
                new Point3D(0.0, 0.0, 0.0),
                0.0,
                new Vector3D(0.0, 0.0, 0.0)));
    }

    private static Mesh3D CreateMesh(double thicknessMillimeters = 3.0)
    {
        return new Mesh3D(
            new[]
            {
                new Point3D(0.0, 0.0, 0.0),
                new Point3D(1.0, 0.0, 0.0),
                new Point3D(0.0, 1.0, thicknessMillimeters),
            },
            new[] { 0, 1, 2 });
    }
}
