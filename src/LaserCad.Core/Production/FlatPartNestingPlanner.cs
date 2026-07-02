using LaserCad.Core.MaterialModel;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.Production;

/// <summary>
/// Adapter ukladajacy plaskie czesci produkcyjne na arkuszach materialu.
/// </summary>
public sealed class FlatPartNestingPlanner
{
    private readonly RowNestingPlanner rowPlanner = new();

    /// <summary>
    /// Zamienia czesci plaskie na prostokatne elementy wejscia nestingu MVP.
    /// </summary>
    public IReadOnlyList<NestingItem> CreateNestingItems(IEnumerable<FlatPart> parts)
    {
        if (parts is null)
        {
            throw new ArgumentNullException(nameof(parts));
        }

        return parts
            .SelectMany(part => Enumerable.Range(1, part.Quantity).Select(index => CreateItem(part, index)))
            .ToArray();
    }

    /// <summary>
    /// Uklada czesci na jednym arkuszu.
    /// </summary>
    public NestingResult NestSingleSheet(
        SheetSize sheetSize,
        IEnumerable<FlatPart> parts,
        NestingOptions? options = null)
    {
        return rowPlanner.Nest(sheetSize, CreateNestingItems(parts), options);
    }

    /// <summary>
    /// Uklada czesci na wielu arkuszach, zaczynajac nowy arkusz, gdy kolejna czesc nie miesci sie na biezacym.
    /// </summary>
    public IReadOnlyList<NestingResult> NestMultipleSheets(
        SheetSize sheetSize,
        IEnumerable<FlatPart> parts,
        NestingOptions? options = null)
    {
        if (sheetSize is null)
        {
            throw new ArgumentNullException(nameof(sheetSize));
        }

        var items = CreateNestingItems(parts);
        var sheets = new List<NestingResult>();
        var currentItems = new List<NestingItem>();

        foreach (var item in items)
        {
            EnsureItemFitsEmptySheet(sheetSize, item, options);

            var candidateItems = currentItems.Concat(new[] { item }).ToArray();
            try
            {
                rowPlanner.Nest(sheetSize, candidateItems, options);
                currentItems.Add(item);
            }
            catch (InvalidOperationException) when (currentItems.Count > 0)
            {
                sheets.Add(rowPlanner.Nest(sheetSize, currentItems, options));
                currentItems.Clear();
                currentItems.Add(item);
            }
        }

        if (currentItems.Count > 0)
        {
            sheets.Add(rowPlanner.Nest(sheetSize, currentItems, options));
        }

        return sheets;
    }

    /// <summary>
    /// Uklada czesci na wielu arkuszach, zachowujac referencje do oryginalnych konturow FlatPart.
    /// </summary>
    public IReadOnlyList<FlatPartSheetNestingResult> NestFlatPartsMultipleSheets(
        SheetSize sheetSize,
        IEnumerable<FlatPart> parts,
        NestingOptions? options = null)
    {
        var partArray = parts?.ToArray() ?? throw new ArgumentNullException(nameof(parts));
        var expandedParts = ExpandParts(partArray).ToArray();
        var sheets = NestMultipleSheets(sheetSize, partArray, options);
        var partIndex = 0;

        return sheets
            .Select((sheet, index) => new FlatPartSheetNestingResult(
                index + 1,
                sheet.Parts.Select(nested =>
                {
                    var sourcePart = expandedParts[partIndex++];
                    return new NestedFlatPart(
                        sourcePart,
                        nested.X,
                        nested.Y,
                        nested.Width,
                        nested.Height,
                        nested.IsRotated);
                })))
            .ToArray();
    }

    private static NestingItem CreateItem(FlatPart part, int index)
    {
        if (part is null)
        {
            throw new ArgumentNullException(nameof(part));
        }

        var bounds = part.OuterContour.Bounds;
        var name = part.Quantity == 1 ? part.Name : $"{part.Name} #{index}";
        return new NestingItem(
            name,
            Length.FromMillimeters(bounds.Width),
            Length.FromMillimeters(bounds.Height));
    }

    private static IEnumerable<FlatPart> ExpandParts(IEnumerable<FlatPart> parts)
    {
        if (parts is null)
        {
            throw new ArgumentNullException(nameof(parts));
        }

        return parts.SelectMany(part => Enumerable.Range(0, part.Quantity).Select(_ => part));
    }

    private void EnsureItemFitsEmptySheet(SheetSize sheetSize, NestingItem item, NestingOptions? options)
    {
        try
        {
            rowPlanner.Nest(sheetSize, new[] { item }, options);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException($"Flat part '{item.Name}' does not fit on an empty sheet.", ex);
        }
    }
}
