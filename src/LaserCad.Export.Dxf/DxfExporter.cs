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
        writer.WriteEndSection();
        writer.WritePair(0, "EOF");

        return writer.ToString();
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

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}
