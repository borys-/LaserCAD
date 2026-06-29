using LaserCad.Core.Documents;

namespace LaserCad.Export.Svg;

/// <summary>
/// Eksportuje dokument CAD do tekstowej reprezentacji SVG.
/// </summary>
public sealed class SvgExporter
{
    /// <summary>
    /// Eksportuje dokument CAD do SVG zgodnie z podanymi opcjami.
    /// </summary>
    public string Export(CadDocument document, SvgExportOptions? options = null)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        options ??= new SvgExportOptions();

        return string.Empty;
    }
}
