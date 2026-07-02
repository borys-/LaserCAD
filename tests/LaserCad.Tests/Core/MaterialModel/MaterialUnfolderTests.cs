using LaserCad.Core.Documents;
using LaserCad.Core.MaterialModel;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.MaterialModel;

public sealed class MaterialUnfolderTests
{
    [Test]
    public void Unfold_WithMaterialSolid_ShouldExtractOuterContour()
    {
        var solid = CreatePlate("Front", 30.0, 20.0);
        var unfolder = new MaterialUnfolder();

        var part = unfolder.Unfold(solid);

        Assert.That(part.Name, Is.EqualTo("Front"));
        Assert.That(part.OuterContour.Bounds.Width, Is.EqualTo(30.0));
        Assert.That(part.OuterContour.Bounds.Height, Is.EqualTo(20.0));
    }

    [Test]
    public void Unfold_WithCutout_ShouldExtractInnerContour()
    {
        var solid = CreatePlate("Panel", 50.0, 40.0)
            .AddCutout(CutoutFeature.Rectangle("Window", new Point2D(10.0, 10.0), 8.0, 6.0), 1.0);
        var unfolder = new MaterialUnfolder();

        var part = unfolder.Unfold(solid);

        Assert.That(part.InnerContours, Has.Count.EqualTo(1));
        Assert.That(part.InnerContours[0].Bounds.Width, Is.EqualTo(8.0));
        Assert.That(part.InnerContours[0].Bounds.Height, Is.EqualTo(6.0));
    }

    [Test]
    public void Unfold_WithDocument_ShouldPreserveTechnologicalLayers()
    {
        var document = new CadDocument()
            .AddMaterialSolid(CreatePlate("Front", 30.0, 20.0));
        var unfolder = new MaterialUnfolder();

        var parts = unfolder.Unfold(document);

        Assert.That(parts, Has.Count.EqualTo(1));
        Assert.That(parts[0].Layers.Select(layer => layer.Role), Is.EqualTo(new[]
        {
            LayerRole.Cut,
            LayerRole.Engrave,
            LayerRole.Score,
            LayerRole.Ignore
        }));
    }

    [Test]
    public void Unfold_WithDocument_ShouldCreatePartsNamedAfterMaterialSolids()
    {
        var document = new CadDocument()
            .AddMaterialSolid(CreatePlate("Left side", 30.0, 20.0))
            .AddMaterialSolid(CreatePlate("Right side", 30.0, 20.0));
        var unfolder = new MaterialUnfolder();

        var parts = unfolder.Unfold(document);

        Assert.That(parts.Select(part => part.Name), Is.EqualTo(new[] { "Left side", "Right side" }));
        Assert.That(parts.SelectMany(part => part.SourceNames), Does.Contain("Left side"));
        Assert.That(parts.SelectMany(part => part.SourceNames), Does.Contain("Right side"));
    }

    [Test]
    public void Unfold_WithMergeIdenticalParts_ShouldDetectDuplicates()
    {
        var document = new CadDocument()
            .AddMaterialSolid(CreatePlate("Left side", 30.0, 20.0))
            .AddMaterialSolid(CreatePlate("Right side", 30.0, 20.0))
            .AddMaterialSolid(CreatePlate("Bottom", 40.0, 20.0));
        var unfolder = new MaterialUnfolder();

        var parts = unfolder.Unfold(document, new MaterialUnfoldingOptions(mergeIdenticalParts: true));

        Assert.That(parts, Has.Count.EqualTo(2));
        Assert.That(parts.Single(part => part.OuterContour.Bounds.Width == 30.0).Quantity, Is.EqualTo(2));
        Assert.That(parts.Single(part => part.OuterContour.Bounds.Width == 30.0).SourceNames, Is.EqualTo(new[]
        {
            "Left side",
            "Right side"
        }));
    }

    [Test]
    public void Unfold_WithSeveralMaterialSolids_ShouldReturnFlatPartList()
    {
        var document = new CadDocument()
            .AddMaterialSolid(CreatePlate("Front", 30.0, 20.0))
            .AddMaterialSolid(CreatePlate("Back", 30.0, 20.0))
            .AddMaterialSolid(CreatePlate("Bottom", 30.0, 15.0));
        var unfolder = new MaterialUnfolder();

        var parts = unfolder.Unfold(document);

        Assert.That(parts, Has.Count.EqualTo(3));
        Assert.That(parts.All(part => part.OuterContour.Area > 0.0), Is.True);
    }

    [Test]
    public void Unfold_WithSlopedMaterialSolid_ShouldCreateNamedFlatParts()
    {
        var solid = new SlopedMaterialSolid(
            "Bryla trapezowa",
            DefaultMaterialProfiles.Plywood3Mm,
            new SlopedMaterialSolidOptions(
                LaserCad.Geometry.Units.Length.FromMillimeters(100.0),
                LaserCad.Geometry.Units.Length.FromMillimeters(60.0),
                LaserCad.Geometry.Units.Length.FromMillimeters(40.0),
                LaserCad.Geometry.Units.Length.FromMillimeters(70.0)));
        var unfolder = new MaterialUnfolder();

        var parts = unfolder.Unfold(solid);

        Assert.That(parts, Has.Count.EqualTo(6));
        Assert.That(parts.Select(part => part.Name), Does.Contain("Bryla trapezowa - Left side"));
        Assert.That(parts.Single(part => part.Name.EndsWith("Left side", StringComparison.Ordinal)).OuterContour.Bounds.Height, Is.EqualTo(70.0));
    }

    private static MaterialSolid CreatePlate(string name, double width, double height)
    {
        var rectangle = new RectangleEntity(new Point2D(0.0, 0.0), width, height);
        return MaterialSolid.FromRectangle(name, rectangle, DefaultMaterialProfiles.Plywood3Mm);
    }
}
