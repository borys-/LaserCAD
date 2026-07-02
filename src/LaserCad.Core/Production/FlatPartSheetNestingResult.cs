namespace LaserCad.Core.Production;

/// <summary>
/// Wynik ulozenia plaskich czesci na pojedynczym arkuszu.
/// </summary>
public sealed class FlatPartSheetNestingResult
{
    /// <summary>
    /// Tworzy wynik arkusza.
    /// </summary>
    public FlatPartSheetNestingResult(int sheetNumber, IEnumerable<NestedFlatPart> parts)
    {
        if (sheetNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(sheetNumber), "Sheet number must be greater than zero.");
        }

        SheetNumber = sheetNumber;
        Parts = parts?.ToArray() ?? throw new ArgumentNullException(nameof(parts));
    }

    /// <summary>
    /// Numer arkusza liczony od 1.
    /// </summary>
    public int SheetNumber { get; }

    /// <summary>
    /// Czesci ulozone na arkuszu.
    /// </summary>
    public IReadOnlyList<NestedFlatPart> Parts { get; }
}
