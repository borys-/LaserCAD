namespace LaserCad.Core.Library;

/// <summary>
/// Szablon generatora wczytany z biblioteki aplikacji.
/// </summary>
public sealed class LibraryTemplate
{
    /// <summary>
    /// Tworzy opis szablonu bibliotecznego.
    /// </summary>
    public LibraryTemplate(
        string id,
        string name,
        string generatorType,
        string description,
        IReadOnlyDictionary<string, object> parameters)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Identyfikator szablonu nie moze byc pusty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Nazwa szablonu nie moze byc pusta.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(generatorType))
        {
            throw new ArgumentException("Typ generatora szablonu nie moze byc pusty.", nameof(generatorType));
        }

        Id = id;
        Name = name;
        GeneratorType = generatorType;
        Description = description ?? string.Empty;
        Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
    }

    /// <summary>
    /// Stabilny identyfikator szablonu.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Nazwa prezentowana uzytkownikowi.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Nazwa generatora domenowego.
    /// </summary>
    public string GeneratorType { get; }

    /// <summary>
    /// Krotki opis przeznaczenia szablonu.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Parametry generatora z pliku JSON.
    /// </summary>
    public IReadOnlyDictionary<string, object> Parameters { get; }

    internal static LibraryTemplate FromDto(TemplateDto dto)
    {
        return new LibraryTemplate(
            dto.Id,
            dto.Name,
            dto.GeneratorType,
            dto.Description,
            dto.Parameters);
    }
}
