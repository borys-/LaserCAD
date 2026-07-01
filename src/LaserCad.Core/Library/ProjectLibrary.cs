namespace LaserCad.Core.Library;

/// <summary>
/// Zbior profili materialow i szablonow dostepnych w bibliotece aplikacji.
/// </summary>
public sealed class ProjectLibrary
{
    /// <summary>
    /// Tworzy biblioteke z wczytanych zasobow.
    /// </summary>
    public ProjectLibrary(
        IEnumerable<LibraryMaterialProfile> materials,
        IEnumerable<LibraryTemplate> templates)
    {
        Materials = (materials ?? throw new ArgumentNullException(nameof(materials))).ToArray();
        Templates = (templates ?? throw new ArgumentNullException(nameof(templates))).ToArray();
    }

    /// <summary>
    /// Profile materialow w kolejnosci nazw plikow.
    /// </summary>
    public IReadOnlyList<LibraryMaterialProfile> Materials { get; }

    /// <summary>
    /// Szablony generatorow w kolejnosci nazw plikow.
    /// </summary>
    public IReadOnlyList<LibraryTemplate> Templates { get; }
}
