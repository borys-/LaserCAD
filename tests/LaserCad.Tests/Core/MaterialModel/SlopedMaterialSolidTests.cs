using LaserCad.Core.Documents;
using LaserCad.Core.MaterialModel;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.MaterialModel;

public sealed class SlopedMaterialSolidTests
{
    [Test]
    public void Constructor_ShouldPreserveMaterialAndOptions()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;
        var options = CreateOptions();

        var solid = new SlopedMaterialSolid("Bryla trapezowa", material, options);

        Assert.That(solid.Name, Is.EqualTo("Bryla trapezowa"));
        Assert.That(solid.MaterialProfile, Is.SameAs(material));
        Assert.That(solid.Options, Is.SameAs(options));
        Assert.That(solid.PreviewMesh.Vertices, Has.Count.EqualTo(8));
        Assert.That(solid.PreviewMesh.TriangleCount, Is.EqualTo(12));
    }

    [Test]
    public void Options_ShouldExposeSlopeParameters()
    {
        var options = CreateOptions(frontHeight: 40.0, backHeight: 70.0, depth: 40.0);

        Assert.That(options.Width.Millimeters, Is.EqualTo(100.0));
        Assert.That(options.Depth.Millimeters, Is.EqualTo(40.0));
        Assert.That(options.FrontHeight.Millimeters, Is.EqualTo(40.0));
        Assert.That(options.BackHeight.Millimeters, Is.EqualTo(70.0));
        Assert.That(options.HeightDelta.Millimeters, Is.EqualTo(30.0));
        Assert.That(options.SlopedDepth.Millimeters, Is.EqualTo(50.0).Within(0.000001));
    }

    [Test]
    public void PreviewMesh_ShouldRepresentOneSlopedTopFace()
    {
        var solid = new SlopedMaterialSolid("Bryla trapezowa", DefaultMaterialProfiles.Plywood3Mm, CreateOptions());

        var frontTop = solid.PreviewMesh.Vertices.Single(vertex =>
            vertex.X == 0.0 && vertex.Y == 0.0 && vertex.Z == 40.0);
        var backTop = solid.PreviewMesh.Vertices.Single(vertex =>
            vertex.X == 0.0 && vertex.Y == 60.0 && vertex.Z == 70.0);

        Assert.That(frontTop.Z, Is.LessThan(backTop.Z));
    }

    [Test]
    public void Options_WithZeroFrontHeight_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CreateOptions(frontHeight: 0.0));
    }

    [Test]
    public void Options_WithNegativeBackHeight_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            CreateOptions(backHeight: -1.0));
    }

    [Test]
    public void Unfold_ShouldCreateTwoTrapezoidSidesAndRectangularPanels()
    {
        var solid = new SlopedMaterialSolid("Bryla trapezowa", DefaultMaterialProfiles.Plywood3Mm, CreateOptions());

        var parts = solid.Unfold();

        Assert.That(parts, Has.Count.EqualTo(6));
        Assert.That(parts.Count(IsTrapezoid), Is.EqualTo(2));
        Assert.That(parts.Count(part => !IsTrapezoid(part)), Is.EqualTo(4));
    }

    [Test]
    public void Unfold_ShouldCreateSideTrapezoidsWithFrontAndBackHeights()
    {
        var solid = new SlopedMaterialSolid("Bryla trapezowa", DefaultMaterialProfiles.Plywood3Mm, CreateOptions());

        var leftSide = solid.Unfold().Single(part => part.Name == "Left side");

        Assert.That(leftSide.OuterContour.Vertices[2], Is.EqualTo(new Point2D(60.0, 70.0)));
        Assert.That(leftSide.OuterContour.Vertices[3], Is.EqualTo(new Point2D(0.0, 40.0)));
    }

    [Test]
    public void Unfold_ShouldUseSlopedTopDepth()
    {
        var solid = new SlopedMaterialSolid(
            "Bryla trapezowa",
            DefaultMaterialProfiles.Plywood3Mm,
            CreateOptions(depth: 40.0, frontHeight: 40.0, backHeight: 70.0));

        var slopedTop = solid.Unfold().Single(part => part.Name == "Sloped top");

        Assert.That(slopedTop.OuterContour.Bounds.Width, Is.EqualTo(100.0));
        Assert.That(slopedTop.OuterContour.Bounds.Height, Is.EqualTo(50.0).Within(0.000001));
    }

    private static SlopedMaterialSolidOptions CreateOptions(
        double width = 100.0,
        double depth = 60.0,
        double frontHeight = 40.0,
        double backHeight = 70.0)
    {
        return new SlopedMaterialSolidOptions(
            Length.FromMillimeters(width),
            Length.FromMillimeters(depth),
            Length.FromMillimeters(frontHeight),
            Length.FromMillimeters(backHeight));
    }

    private static bool IsTrapezoid(UnfoldedMaterialPart part)
    {
        var vertices = part.OuterContour.Vertices;

        return Math.Abs(vertices[2].Y - vertices[3].Y) > 0.000001;
    }
}
