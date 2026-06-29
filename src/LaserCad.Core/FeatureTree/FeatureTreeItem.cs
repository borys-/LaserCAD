using LaserCad.Core.Documents;

namespace LaserCad.Core.FeatureTree;

/// <summary>
/// Bazowy element drzewa historii modelu.
/// </summary>
public abstract class FeatureTreeItem
{
    protected FeatureTreeItem(Guid? id, string name, FeatureTreeItemKind kind, bool isEnabled = true)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Feature tree item name cannot be empty.", nameof(name));
        }

        Id = id ?? Guid.NewGuid();
        Name = name;
        Kind = kind;
        IsEnabled = isEnabled;

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Feature tree item id cannot be empty.", nameof(id));
        }
    }

    /// <summary>
    /// Stabilny identyfikator wpisu drzewa historii.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Nazwa wpisu prezentowana uzytkownikowi.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Rodzaj wpisu drzewa historii.
    /// </summary>
    public FeatureTreeItemKind Kind { get; }

    /// <summary>
    /// Okresla, czy wpis bierze udzial w przebudowie dokumentu.
    /// </summary>
    public bool IsEnabled { get; }

    /// <summary>
    /// Zwraca kopie wpisu z ustawiona aktywnoscia.
    /// </summary>
    public abstract FeatureTreeItem WithEnabled(bool isEnabled);

    /// <summary>
    /// Stosuje wpis do dokumentu podczas przebudowy drzewa historii.
    /// </summary>
    public abstract CadDocument Apply(CadDocument document);
}
