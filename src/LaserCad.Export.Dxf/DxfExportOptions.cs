namespace LaserCad.Export.Dxf;

/// <summary>
/// Opcje sterujace eksportem dokumentu CAD do formatu DXF.
/// </summary>
public sealed class DxfExportOptions
{
    private IReadOnlyCollection<string>? _exportedLayerNames;

    /// <summary>
    /// Jednostka dlugosci uzywana w eksporcie.
    /// </summary>
    public DxfExportUnit Unit { get; init; } = DxfExportUnit.Millimeters;

    /// <summary>
    /// Opcjonalna lista nazw warstw eksportowanych do DXF.
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
