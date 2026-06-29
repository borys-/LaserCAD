namespace LaserCad.Export.Svg;

/// <summary>
/// Opcje sterujace eksportem dokumentu CAD do formatu SVG.
/// </summary>
public sealed class SvgExportOptions
{
    private IReadOnlyCollection<string>? _exportedLayerNames;
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

    /// <summary>
    /// Opcjonalna lista nazw warstw eksportowanych do SVG.
    /// Pusta wartosc oznacza eksport wszystkich warstw.
    /// </summary>
    public IReadOnlyCollection<string>? ExportedLayerNames
    {
        get => _exportedLayerNames;
        init
        {
            if (value is null)
            {
                _exportedLayerNames = null;
                return;
            }

            _exportedLayerNames = value
                .Where(layerName => !string.IsNullOrWhiteSpace(layerName))
                .Select(layerName => layerName.Trim())
                .Distinct(StringComparer.Ordinal)
                .ToArray();
        }
    }
}
