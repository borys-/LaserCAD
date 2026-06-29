using LaserCad.Core.Documents;
using System.Globalization;
using System.Text;

namespace LaserCad.Export.Dxf;

/// <summary>
/// Eksportuje dokument CAD do tekstowej reprezentacji DXF.
/// </summary>
public sealed class DxfExporter
{
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

        var writer = new DxfWriter();
        writer.WriteSection("HEADER");
        writer.WriteEndSection();
        writer.WriteSection("TABLES");
        writer.WriteEndSection();
        writer.WriteSection("ENTITIES");
        foreach (Entity entity in document.Sketches.SelectMany(sketch => sketch.Entities))
        {
            WriteEntity(writer, entity);
        }

        writer.WriteEndSection();
        writer.WritePair(0, "EOF");

        return writer.ToString();
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
