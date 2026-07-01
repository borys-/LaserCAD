using LaserCad.Geometry.Units;

namespace LaserCad.Core.Production;

/// <summary>
/// Prosty nesting ukladajacy prostokatne elementy kolejnymi rzedami.
/// </summary>
public sealed class RowNestingPlanner
{
    /// <summary>
    /// Uklada elementy na arkuszu.
    /// </summary>
    public NestingResult Nest(SheetSize sheetSize, IEnumerable<NestingItem> items, NestingOptions? options = null)
    {
        if (sheetSize is null)
        {
            throw new ArgumentNullException(nameof(sheetSize));
        }

        if (items is null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        options ??= new NestingOptions();

        var parts = new List<NestedPart>();
        var x = sheetSize.Margin.Millimeters;
        var y = sheetSize.Margin.Millimeters;
        var rowHeight = 0.0;
        var usableMaxX = sheetSize.Width.Millimeters - sheetSize.Margin.Millimeters;
        var usableMaxY = sheetSize.Height.Millimeters - sheetSize.Margin.Millimeters;
        var spacing = options.Spacing.Millimeters;

        foreach (var item in items)
        {
            var placement = ChoosePlacement(item, options.AllowRotation, usableMaxX - sheetSize.Margin.Millimeters);

            if (x + placement.width > usableMaxX && x > sheetSize.Margin.Millimeters)
            {
                x = sheetSize.Margin.Millimeters;
                y += rowHeight + spacing;
                rowHeight = 0.0;
            }

            if (x + placement.width > usableMaxX || y + placement.height > usableMaxY)
            {
                throw new InvalidOperationException($"Nesting item '{item.Name}' does not fit on the sheet.");
            }

            parts.Add(new NestedPart(
                item,
                Length.FromMillimeters(x),
                Length.FromMillimeters(y),
                Length.FromMillimeters(placement.width),
                Length.FromMillimeters(placement.height),
                placement.isRotated));

            x += placement.width + spacing;
            rowHeight = Math.Max(rowHeight, placement.height);
        }

        return new NestingResult(parts);
    }

    private static (double width, double height, bool isRotated) ChoosePlacement(
        NestingItem item,
        bool allowRotation,
        double availableRowWidth)
    {
        var width = item.Width.Millimeters;
        var height = item.Height.Millimeters;

        if (allowRotation && height <= availableRowWidth && height < width)
        {
            return (height, width, true);
        }

        return (width, height, false);
    }
}
