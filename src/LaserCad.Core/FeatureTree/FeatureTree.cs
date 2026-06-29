using LaserCad.Core.Documents;

namespace LaserCad.Core.FeatureTree;

/// <summary>
/// Niemutowalne drzewo historii modelu z uporzadkowana lista generatorow i operacji.
/// </summary>
public sealed class FeatureTree
{
    public FeatureTree(IEnumerable<FeatureTreeItem>? items = null)
    {
        Items = items?.ToArray() ?? Array.Empty<FeatureTreeItem>();

        if (Items.Any(item => item is null))
        {
            throw new ArgumentException("Feature tree items cannot contain null values.", nameof(items));
        }
    }

    /// <summary>
    /// Wpisy drzewa historii w kolejnosci przebudowy.
    /// </summary>
    public IReadOnlyList<FeatureTreeItem> Items { get; }

    /// <summary>
    /// Zwraca nowe drzewo z dodanym wpisem na koncu historii.
    /// </summary>
    public FeatureTree Add(FeatureTreeItem item)
    {
        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        return new FeatureTree(Items.Append(item));
    }

    /// <summary>
    /// Zwraca nowe drzewo z aktywowanym wpisem o podanym identyfikatorze.
    /// </summary>
    public FeatureTree Enable(Guid itemId)
    {
        return SetEnabled(itemId, true);
    }

    /// <summary>
    /// Zwraca nowe drzewo z dezaktywowanym wpisem o podanym identyfikatorze.
    /// </summary>
    public FeatureTree Disable(Guid itemId)
    {
        return SetEnabled(itemId, false);
    }

    /// <summary>
    /// Przebudowuje dokument przez wykonanie aktywnych wpisow drzewa historii w kolejnosci.
    /// </summary>
    public CadDocument Rebuild(CadDocument baseDocument)
    {
        if (baseDocument is null)
        {
            throw new ArgumentNullException(nameof(baseDocument));
        }

        var rebuilt = baseDocument;

        foreach (var item in Items)
        {
            if (!item.IsEnabled)
            {
                continue;
            }

            rebuilt = item.Apply(rebuilt);
        }

        return rebuilt;
    }

    private FeatureTree SetEnabled(Guid itemId, bool isEnabled)
    {
        if (itemId == Guid.Empty)
        {
            throw new ArgumentException("Feature tree item id cannot be empty.", nameof(itemId));
        }

        var found = false;
        var items = Items
            .Select(item =>
            {
                if (item.Id != itemId)
                {
                    return item;
                }

                found = true;
                return item.WithEnabled(isEnabled);
            })
            .ToArray();

        if (!found)
        {
            throw new InvalidOperationException($"Feature tree item '{itemId}' was not found.");
        }

        return new FeatureTree(items);
    }
}
