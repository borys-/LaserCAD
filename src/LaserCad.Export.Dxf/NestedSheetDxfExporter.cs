using System.Text.RegularExpressions;
using LaserCad.Core.Documents;
using LaserCad.Core.Production;
using LaserCad.Geometry;

namespace LaserCad.Export.Dxf;

/// <summary>
/// Eksportuje wynik nestingu plaskich czesci do DXF.
/// </summary>
public sealed class NestedSheetDxfExporter
{
    private readonly DxfExporter dxfExporter = new();

    /// <summary>
    /// Eksportuje osobny DXF dla kazdego arkusza oraz jeden zbiorczy DXF.
    /// </summary>
    public NestedSheetDxfExportResult Export(
        string projectName,
        SheetSize sheetSize,
        IReadOnlyList<FlatPartSheetNestingResult> sheets,
        DxfExportOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(projectName))
        {
            throw new ArgumentException("Project name cannot be empty.", nameof(projectName));
        }

        if (sheetSize is null)
        {
            throw new ArgumentNullException(nameof(sheetSize));
        }

        if (sheets is null)
        {
            throw new ArgumentNullException(nameof(sheets));
        }

        var sheetFiles = sheets.ToDictionary(
            sheet => CreateSheetFileName(projectName, sheet.SheetNumber),
            sheet => dxfExporter.Export(CreateSheetDocument(projectName, new[] { sheet }, sheetSize, 0.0), options),
            StringComparer.Ordinal);

        var combined = dxfExporter.Export(
            CreateSheetDocument(projectName, sheets, sheetSize, 20.0),
            options);

        return new NestedSheetDxfExportResult(sheetFiles, combined);
    }

    private static CadDocument CreateSheetDocument(
        string projectName,
        IReadOnlyList<FlatPartSheetNestingResult> sheets,
        SheetSize sheetSize,
        double sheetGapMillimeters)
    {
        var document = new CadDocument(
            name: projectName + " - nested DXF",
            layers: DefaultLayers.All);
        var sketch = new Sketch(name: "Nested sheets");

        foreach (var sheet in sheets)
        {
            var offsetX = (sheet.SheetNumber - 1) * (sheetSize.Width.Millimeters + sheetGapMillimeters);
            foreach (var part in sheet.Parts)
            {
                sketch = AddPolygon(sketch, part.Part.OuterContour, part, offsetX, "Cut");
                foreach (var innerContour in part.Part.InnerContours)
                {
                    sketch = AddPolygon(sketch, innerContour, part, offsetX, "Cut");
                }
            }
        }

        return document.AddSketch(sketch);
    }

    private static Sketch AddPolygon(
        Sketch sketch,
        Polygon2D polygon,
        NestedFlatPart placement,
        double sheetOffsetX,
        string layerName)
    {
        var points = polygon.Vertices
            .Select(point => TransformPoint(point, polygon.Bounds, placement, sheetOffsetX))
            .ToArray();

        return sketch.AddEntity(new PolylineEntity(new Polyline2D(points, isClosed: true), layerName: layerName));
    }

    private static Point2D TransformPoint(
        Point2D point,
        BoundingBox bounds,
        NestedFlatPart placement,
        double sheetOffsetX)
    {
        var localX = point.X - bounds.MinX;
        var localY = point.Y - bounds.MinY;

        if (placement.IsRotated)
        {
            return new Point2D(
                sheetOffsetX + placement.X.Millimeters + localY,
                placement.Y.Millimeters + bounds.Width - localX);
        }

        return new Point2D(
            sheetOffsetX + placement.X.Millimeters + localX,
            placement.Y.Millimeters + localY);
    }

    private static string CreateSheetFileName(string projectName, int sheetNumber)
    {
        var safeName = Regex.Replace(projectName.Trim(), "[^A-Za-z0-9_.-]+", "-");
        return $"{safeName}-sheet-{sheetNumber:000}.dxf";
    }
}
