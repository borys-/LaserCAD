using LaserCad.Core.Documents;
using LaserCad.Core.MaterialModel;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.MaterialModel;

public sealed class CutoutFeatureTests
{
    [Test]
    public void Rectangle_ShouldCreateRectangularContour()
    {
        var cutout = CutoutFeature.Rectangle("Okno", new Point2D(10.0, 10.0), 20.0, 15.0);

        Assert.That(cutout.Kind, Is.EqualTo(CutoutFeatureKind.Rectangle));
        Assert.That(cutout.Contour.Vertices, Has.Count.EqualTo(4));
        Assert.That(cutout.Bounds.Width, Is.EqualTo(20.0));
        Assert.That(cutout.Bounds.Height, Is.EqualTo(15.0));
    }

    [Test]
    public void Circle_ShouldCreateCircularPolygonContour()
    {
        var cutout = CutoutFeature.Circle("Otwor", new Point2D(30.0, 30.0), 5.0);

        Assert.That(cutout.Kind, Is.EqualTo(CutoutFeatureKind.Circle));
        Assert.That(cutout.Contour.Vertices, Has.Count.EqualTo(48));
        Assert.That(cutout.Bounds.MinX, Is.EqualTo(25.0).Within(0.000001));
        Assert.That(cutout.Bounds.MaxX, Is.EqualTo(35.0).Within(0.000001));
    }

    [Test]
    public void FromClosedPolyline_ShouldCreatePolylineCutout()
    {
        var polyline = new Polyline2D(
            new[]
            {
                new Point2D(10.0, 10.0),
                new Point2D(20.0, 10.0),
                new Point2D(20.0, 20.0),
                new Point2D(10.0, 20.0),
            },
            isClosed: true);

        var cutout = CutoutFeature.FromClosedPolyline("Ksztalt", polyline);

        Assert.That(cutout.Kind, Is.EqualTo(CutoutFeatureKind.Polyline));
        Assert.That(cutout.Contour.Vertices, Is.EqualTo(polyline.Points));
    }

    [Test]
    public void FromClosedPolyline_WithOpenPolyline_ShouldThrow()
    {
        var polyline = new Polyline2D(new[] { new Point2D(0.0, 0.0), new Point2D(1.0, 0.0), new Point2D(1.0, 1.0) });

        Assert.Throws<ArgumentException>(() => CutoutFeature.FromClosedPolyline("Ksztalt", polyline));
    }

    [Test]
    public void MaterialSolid_AddCutoutInsidePlate_ShouldPreserveCutout()
    {
        var solid = CreatePlate();
        var cutout = CutoutFeature.Rectangle("Okno", new Point2D(20.0, 20.0), 10.0, 10.0);

        var updated = solid.AddCutout(cutout, minimumBridgeMillimeters: 5.0);

        Assert.That(updated.Cutouts.Single(), Is.SameAs(cutout));
        Assert.That(solid.Cutouts, Is.Empty);
    }

    [Test]
    public void MaterialSolid_AddCutoutOutsidePlate_ShouldThrow()
    {
        var solid = CreatePlate();
        var cutout = CutoutFeature.Rectangle("Okno", new Point2D(95.0, 20.0), 10.0, 10.0);

        Assert.Throws<ArgumentException>(() => solid.AddCutout(cutout, minimumBridgeMillimeters: 1.0));
    }

    [Test]
    public void MaterialSolid_AddCutoutWithTooSmallBridge_ShouldThrow()
    {
        var solid = CreatePlate();
        var cutout = CutoutFeature.Rectangle("Okno", new Point2D(2.0, 20.0), 10.0, 10.0);

        Assert.Throws<ArgumentException>(() => solid.AddCutout(cutout, minimumBridgeMillimeters: 5.0));
    }

    [Test]
    public void MaterialSolid_CreateFlatPreview_ShouldIncludeCutoutAsInnerContour()
    {
        var solid = CreatePlate()
            .AddCutout(CutoutFeature.Circle("Otwor", new Point2D(50.0, 30.0), 5.0), minimumBridgeMillimeters: 5.0);

        var preview = solid.CreateFlatPreview();

        Assert.That(preview.OuterContour.Bounds.Width, Is.EqualTo(100.0));
        Assert.That(preview.OuterContour.Bounds.Height, Is.EqualTo(60.0));
        Assert.That(preview.InnerContours, Has.Count.EqualTo(1));
        Assert.That(preview.InnerContours[0].Bounds.Width, Is.EqualTo(10.0).Within(0.000001));
    }

    private static MaterialSolid CreatePlate()
    {
        return MaterialSolid.FromRectangle(
            "Plyta",
            new RectangleEntity(new Point2D(0.0, 0.0), 100.0, 60.0),
            DefaultMaterialProfiles.Plywood3Mm);
    }
}
