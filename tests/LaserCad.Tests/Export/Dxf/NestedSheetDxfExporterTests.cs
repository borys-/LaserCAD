using LaserCad.Core.MaterialModel;
using LaserCad.Core.Production;
using LaserCad.Export.Dxf;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Export.Dxf;

public sealed class NestedSheetDxfExporterTests
{
    [Test]
    public void Export_ShouldCreateSeparateSheetFilesAndCombinedDxf()
    {
        var sheets = CreateNestedSheets(new[]
        {
            new FlatPart("Front", CreateRectangle(30.0, 20.0)),
            new FlatPart("Back", CreateRectangle(30.0, 20.0)),
        });
        var exporter = new NestedSheetDxfExporter();

        var result = exporter.Export("Projekt testowy", CreateSheet(), sheets);

        Assert.That(result.SheetFiles.Keys.Single(), Is.EqualTo("Projekt-testowy-sheet-001.dxf"));
        Assert.That(result.SheetFiles.Values.Single(), Does.Contain("0\r\nLWPOLYLINE\r\n"));
        Assert.That(result.CombinedDxf, Does.Contain("0\r\nLWPOLYLINE\r\n"));
    }

    [Test]
    public void Export_ShouldPreserveCutEngraveAndScoreLayers()
    {
        var sheets = CreateNestedSheets(new[] { new FlatPart("Front", CreateRectangle(30.0, 20.0)) });
        var exporter = new NestedSheetDxfExporter();

        var result = exporter.Export("Project", CreateSheet(), sheets);

        Assert.That(result.CombinedDxf, Does.Contain("2\r\nCut\r\n"));
        Assert.That(result.CombinedDxf, Does.Contain("2\r\nEngrave\r\n"));
        Assert.That(result.CombinedDxf, Does.Contain("2\r\nScore\r\n"));
        Assert.That(result.CombinedDxf, Does.Not.Contain("2\r\nIgnore\r\n"));
    }

    [Test]
    public void Export_WithInnerContour_ShouldWriteCutPolylineForHole()
    {
        var part = new FlatPart(
            "Panel",
            CreateRectangle(50.0, 40.0),
            new[] { CreateRectangle(8.0, 6.0, 10.0, 10.0) });
        var sheets = CreateNestedSheets(new[] { part });
        var exporter = new NestedSheetDxfExporter();

        var result = exporter.Export("Project", CreateSheet(), sheets);

        Assert.That(CountOccurrences(result.CombinedDxf, "0\r\nLWPOLYLINE\r\n"), Is.EqualTo(2));
        Assert.That(result.CombinedDxf, Does.Contain("8\r\nCut\r\n90\r\n4\r\n70\r\n1\r\n"));
    }

    [Test]
    public void Export_WithSeveralPlatesAndHole_ShouldWriteRegressionDxf()
    {
        var parts = new[]
        {
            new FlatPart("Front", CreateRectangle(50.0, 40.0), new[] { CreateRectangle(8.0, 6.0, 10.0, 10.0) }),
            new FlatPart("Back", CreateRectangle(50.0, 40.0)),
            new FlatPart("Bottom", CreateRectangle(60.0, 30.0)),
        };
        var sheets = CreateNestedSheets(parts);
        var exporter = new NestedSheetDxfExporter();

        var result = exporter.Export("Model 3D", CreateSheet(), sheets);

        Assert.That(result.SheetFiles, Has.Count.EqualTo(1));
        Assert.That(CountOccurrences(result.CombinedDxf, "0\r\nLWPOLYLINE\r\n"), Is.EqualTo(4));
        Assert.That(result.CombinedDxf, Does.Contain("2\r\nCut\r\n"));
    }

    private static IReadOnlyList<FlatPartSheetNestingResult> CreateNestedSheets(IReadOnlyList<FlatPart> parts)
    {
        return new FlatPartNestingPlanner().NestFlatPartsMultipleSheets(
            CreateSheet(),
            parts,
            new NestingOptions(spacing: Length.FromMillimeters(5.0)));
    }

    private static SheetSize CreateSheet()
    {
        return new SheetSize(
            Length.FromMillimeters(300.0),
            Length.FromMillimeters(300.0),
            Length.FromMillimeters(5.0));
    }

    private static Polygon2D CreateRectangle(double width, double height, double x = 0.0, double y = 0.0)
    {
        return new Polygon2D(new[]
        {
            new Point2D(x, y),
            new Point2D(x + width, y),
            new Point2D(x + width, y + height),
            new Point2D(x, y + height)
        });
    }

    private static int CountOccurrences(string text, string value)
    {
        var count = 0;
        var index = 0;
        while ((index = text.IndexOf(value, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }
}
