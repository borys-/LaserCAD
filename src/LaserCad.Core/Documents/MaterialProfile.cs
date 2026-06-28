namespace LaserCad.Core.Documents;

/// <summary>
/// Reprezentuje profil materialu przypisany do dokumentu.
/// Szczegolowe parametry materialu beda rozbudowywane w kolejnej sekcji roadmapy.
/// </summary>
public sealed class MaterialProfile
{
    /// <summary>
    /// Tworzy profil materialu o podanej nazwie.
    /// Uzywaj nazw opisowych, np. "Plywood 3 mm" albo "Acrylic".
    /// </summary>
    public MaterialProfile(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Material profile name cannot be empty.", nameof(name));
        }

        Name = name;
    }

    /// <summary>
    /// Nazwa materialu prezentowana uzytkownikowi.
    /// </summary>
    public string Name { get; }
}
