namespace LaserCad.Export.Dxf;

/// <summary>
/// Wynik eksportu DXF arkuszy ulozonych przez nesting.
/// </summary>
public sealed class NestedSheetDxfExportResult
{
    /// <summary>
    /// Tworzy wynik eksportu arkuszy.
    /// </summary>
    public NestedSheetDxfExportResult(IReadOnlyDictionary<string, string> sheetFiles, string combinedDxf)
    {
        SheetFiles = sheetFiles ?? throw new ArgumentNullException(nameof(sheetFiles));
        CombinedDxf = combinedDxf ?? throw new ArgumentNullException(nameof(combinedDxf));
    }

    /// <summary>
    /// Osobne pliki DXF, gdzie kluczem jest nazwa pliku.
    /// </summary>
    public IReadOnlyDictionary<string, string> SheetFiles { get; }

    /// <summary>
    /// Jeden zbiorczy DXF z arkuszami ulozonymi obok siebie.
    /// </summary>
    public string CombinedDxf { get; }
}
