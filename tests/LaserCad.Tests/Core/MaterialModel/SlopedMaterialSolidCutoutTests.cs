using LaserCad.Core.Documents;
using LaserCad.Core.MaterialModel;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.MaterialModel;

public sealed class SlopedMaterialSolidCutoutTests
{
    [Test]
    public void AddCutout_ShouldAllowCircularHolesOnTwoDifferentFaces()
    {
        var solid = CreateSolid()
            .AddCutout(CutoutFeature.Circle("Otwor lewy", new Point2D(20.0, 20.0), 3.0, "Left side"), 2.0)
            .AddCutout(CutoutFeature.Circle("Otwor prawy", new Point2D(25.0, 25.0), 4.0, "Right side"), 2.0);

        Assert.That(solid.Cutouts, Has.Count.EqualTo(2));
        Assert.That(solid.Cutouts.Select(cutout => cutout.FaceName), Is.EqualTo(new[] { "Left side", "Right side" }));
    }

    [Test]
    public void AddCutout_OnTrapezoidFaceOutsideUnfoldedContour_ShouldThrow()
    {
        var solid = CreateSolid();
        var cutout = CutoutFeature.Circle("Za wysoko", new Point2D(10.0, 60.0), 2.0, "Left side");

        Assert.Throws<ArgumentException>(() => solid.AddCutout(cutout, 1.0));
    }

    [Test]
    public void Unfold_WithCutoutsOnTwoFaces_ShouldExposeBothInternalContours()
    {
        var solid = CreateSolid()
            .AddCutout(CutoutFeature.Circle("Otwor lewy", new Point2D(20.0, 20.0), 3.0, "Left side"), 2.0)
            .AddCutout(CutoutFeature.Circle("Otwor prawy", new Point2D(25.0, 25.0), 4.0, "Right side"), 2.0);

        var parts = solid.Unfold();
        var leftSide = parts.Single(part => part.Name == "Left side");
        var rightSide = parts.Single(part => part.Name == "Right side");

        Assert.That(leftSide.InnerContours, Has.Count.EqualTo(1));
        Assert.That(rightSide.InnerContours, Has.Count.EqualTo(1));
        Assert.That(parts.Sum(part => part.InnerContours.Count), Is.EqualTo(2));
    }

    private static SlopedMaterialSolid CreateSolid()
    {
        return new SlopedMaterialSolid(
            "Bryla trapezowa",
            DefaultMaterialProfiles.Plywood3Mm,
            new SlopedMaterialSolidOptions(
                Length.FromMillimeters(100.0),
                Length.FromMillimeters(60.0),
                Length.FromMillimeters(40.0),
                Length.FromMillimeters(70.0)));
    }
}
