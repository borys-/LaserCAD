namespace LaserCad.Export.Svg;

/// <summary>
/// Opcje sterujace eksportem dokumentu CAD do formatu SVG.
/// </summary>
public sealed class SvgExportOptions
{
    /// <summary>
    /// Jednostka dlugosci uzywana w eksporcie.
    /// </summary>
    public SvgExportUnit Unit { get; init; } = SvgExportUnit.Millimeters;
}
