using LaserCad.Core.Documents;
using LaserCad.Geometry;
using System.Globalization;
using System.Text;

namespace LaserCad.Export.Dxf;

/// <summary>
/// Eksportuje dokument CAD do tekstowej reprezentacji DXF.
/// </summary>
public sealed class DxfExporter
{
    private const double FullTurnDegrees = 360.0;

    /// <summary>
    /// Eksportuje dokument CAD do DXF zgodnie z podanymi opcjami.
    /// </summary>
    public string Export(CadDocument document, DxfExportOptions? options = null)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        options ??= new DxfExportOptions();
        var layersByName = document.Layers.ToDictionary(layer => layer.Name, StringComparer.Ordinal);

        var writer = new DxfWriter();
        writer.WriteSection("HEADER");
        writer.WriteEndSection();
        writer.WriteSection("TABLES");
        WriteLayerTable(writer, document.Layers, options);
        writer.WriteEndSection();
        writer.WriteSection("ENTITIES");
        foreach (Entity entity in document.Sketches.SelectMany(sketch => sketch.Entities))
        {
            if (!ShouldExportEntity(entity, options, layersByName))
            {
                continue;
            }

            WriteEntity(writer, entity);
        }

        writer.WriteEndSection();
        writer.WritePair(0, "EOF");

        return writer.ToString();
    }

    private static void WriteLayerTable(DxfWriter writer, IReadOnlyList<Layer> layers, DxfExportOptions options)
    {
        Layer[] exportedLayers = layers
            .Where(layer => layer.Role != LayerRole.Ignore)
            .Where(layer => ShouldExportLayer(layer.Name, options))
            .ToArray();

        writer.WritePair(0, "TABLE");
        writer.WritePair(2, "LAYER");
        writer.WritePair(70, exportedLayers.Length.ToString(CultureInfo.InvariantCulture));

        foreach (Layer layer in exportedLayers)
        {
            writer.WritePair(0, "LAYER");
            writer.WritePair(2, layer.Name);
            writer.WritePair(70, "0");
            writer.WritePair(62, GetDxfColorIndex(layer.Color).ToString(CultureInfo.InvariantCulture));
            writer.WritePair(6, "CONTINUOUS");
        }

        writer.WritePair(0, "ENDTAB");
    }

    private static bool ShouldExportEntity(
        Entity entity,
        DxfExportOptions options,
        IReadOnlyDictionary<string, Layer> layersByName)
    {
        if (layersByName.TryGetValue(entity.LayerName, out var layer) && layer.Role == LayerRole.Ignore)
        {
            return false;
        }

        return ShouldExportLayer(entity.LayerName, options);
    }

    private static bool ShouldExportLayer(string layerName, DxfExportOptions options)
    {
        return options.ExportedLayerNames is null
            || options.ExportedLayerNames.Any(exportedLayerName => string.Equals(exportedLayerName, layerName, StringComparison.Ordinal));
    }

    private static int GetDxfColorIndex(LayerColor color)
    {
        return color.Hex switch
        {
            "#FF0000" => 1,
            "#FFFF00" => 2,
            "#00FF00" or "#00AA00" => 3,
            "#00FFFF" => 4,
            "#0000FF" => 5,
            "#FF00FF" => 6,
            "#808080" => 8,
            _ => 7,
        };
    }

    private static void WriteEntity(DxfWriter writer, Entity entity)
    {
        if (entity is LineEntity line)
        {
            WriteLine(writer, line);
        }
        else if (entity is CircleEntity circle)
        {
            WriteCircle(writer, circle);
        }
        else if (entity is ArcEntity arc)
        {
            WriteArc(writer, arc);
        }
        else if (entity is PolylineEntity polyline)
        {
            WritePolyline(writer, polyline.LayerName, polyline.Polyline.Points, polyline.Polyline.IsClosed);
        }
        else if (entity is RectangleEntity rectangle)
        {
            WritePolyline(writer, rectangle.LayerName, rectangle.Corners, isClosed: true);
        }
    }

    private static void WriteLine(DxfWriter writer, LineEntity entity)
    {
        writer.WritePair(0, "LINE");
        writer.WritePair(8, entity.LayerName);
        writer.WritePair(10, entity.Segment.Start.X);
        writer.WritePair(20, entity.Segment.Start.Y);
        writer.WritePair(11, entity.Segment.End.X);
        writer.WritePair(21, entity.Segment.End.Y);
    }

    private static void WriteCircle(DxfWriter writer, CircleEntity entity)
    {
        writer.WritePair(0, "CIRCLE");
        writer.WritePair(8, entity.LayerName);
        writer.WritePair(10, entity.Circle.Center.X);
        writer.WritePair(20, entity.Circle.Center.Y);
        writer.WritePair(40, entity.Circle.Radius);
    }

    private static void WriteArc(DxfWriter writer, ArcEntity entity)
    {
        double startAngleDegrees = ToDxfAngleDegrees(entity.Arc.StartAngleRadians);
        double endAngleDegrees = ToDxfAngleDegrees(entity.Arc.EndAngleRadians);

        if (entity.Arc.Direction == ArcDirection.Clockwise)
        {
            (startAngleDegrees, endAngleDegrees) = (endAngleDegrees, startAngleDegrees);
        }

        writer.WritePair(0, "ARC");
        writer.WritePair(8, entity.LayerName);
        writer.WritePair(10, entity.Arc.Center.X);
        writer.WritePair(20, entity.Arc.Center.Y);
        writer.WritePair(40, entity.Arc.Radius);
        writer.WritePair(50, startAngleDegrees);
        writer.WritePair(51, endAngleDegrees);
    }

    private static void WritePolyline(
        DxfWriter writer,
        string layerName,
        IReadOnlyList<Point2D> points,
        bool isClosed)
    {
        writer.WritePair(0, "LWPOLYLINE");
        writer.WritePair(8, layerName);
        writer.WritePair(90, points.Count.ToString(CultureInfo.InvariantCulture));
        writer.WritePair(70, isClosed ? "1" : "0");

        foreach (Point2D point in points)
        {
            writer.WritePair(10, point.X);
            writer.WritePair(20, point.Y);
        }
    }

    private static double ToDxfAngleDegrees(double angleRadians)
    {
        double angleDegrees = angleRadians * 180.0 / Math.PI;
        double normalized = angleDegrees % FullTurnDegrees;

        if (normalized < 0.0)
        {
            normalized += FullTurnDegrees;
        }

        return normalized;
    }

    private sealed class DxfWriter
    {
        private readonly StringBuilder _builder = new();

        public void WriteSection(string name)
        {
            WritePair(0, "SECTION");
            WritePair(2, name);
        }

        public void WriteEndSection()
        {
            WritePair(0, "ENDSEC");
        }

        public void WritePair(int groupCode, string value)
        {
            _builder.Append(groupCode.ToString(CultureInfo.InvariantCulture));
            _builder.AppendLine();
            _builder.Append(value);
            _builder.AppendLine();
        }

        public void WritePair(int groupCode, double value)
        {
            WritePair(groupCode, value.ToString("0.######", CultureInfo.InvariantCulture));
        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
