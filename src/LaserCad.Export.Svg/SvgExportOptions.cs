namespace LaserCad.Export.Svg;

/// <summary>
/// Opcje sterujace eksportem dokumentu CAD do formatu SVG.
/// </summary>
public sealed class SvgExportOptions
{
    private double _strokeWidthMillimeters = 0.1;

    /// <summary>
    /// Jednostka dlugosci uzywana w eksporcie.
    /// </summary>
    public SvgExportUnit Unit { get; init; } = SvgExportUnit.Millimeters;

    /// <summary>
    /// Grubosc linii w eksportowanym SVG, wyrazona w milimetrach.
    /// </summary>
    public double StrokeWidthMillimeters
    {
        get => _strokeWidthMillimeters;
        init
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Grubosc linii SVG nie moze byc ujemna.");
            }

            _strokeWidthMillimeters = value;
        }
    }
}
