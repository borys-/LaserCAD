using System.Text.Json;

namespace LaserCad.Core.Library;

/// <summary>
/// Wczytuje biblioteke materialow i szablonow z katalogu aplikacji.
/// </summary>
public sealed class ProjectLibraryLoader
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Wczytuje biblioteke z katalogu zawierajacego podkatalogi `materials` i `templates`.
    /// </summary>
    public ProjectLibrary Load(string libraryDirectory)
    {
        if (string.IsNullOrWhiteSpace(libraryDirectory))
        {
            throw new ArgumentException("Sciezka biblioteki nie moze byc pusta.", nameof(libraryDirectory));
        }

        var materials = LoadMaterials(Path.Combine(libraryDirectory, "materials"));
        var templates = LoadTemplates(Path.Combine(libraryDirectory, "templates"));
        return new ProjectLibrary(materials, templates);
    }

    private static IReadOnlyList<LibraryMaterialProfile> LoadMaterials(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return Array.Empty<LibraryMaterialProfile>();
        }

        return Directory.EnumerateFiles(directory, "*.material.json")
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .Select(ReadMaterial)
            .ToArray();
    }

    private static IReadOnlyList<LibraryTemplate> LoadTemplates(string directory)
    {
        if (!Directory.Exists(directory))
        {
            return Array.Empty<LibraryTemplate>();
        }

        return Directory.EnumerateFiles(directory, "*.template.json")
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .Select(ReadTemplate)
            .ToArray();
    }

    private static LibraryMaterialProfile ReadMaterial(string path)
    {
        var dto = JsonSerializer.Deserialize<MaterialProfileDto>(File.ReadAllText(path), SerializerOptions)
            ?? throw new InvalidOperationException("Nie mozna wczytac profilu materialu: " + path);
        dto.Validate(path);
        return LibraryMaterialProfile.FromDto(dto);
    }

    private static LibraryTemplate ReadTemplate(string path)
    {
        var dto = JsonSerializer.Deserialize<TemplateDto>(File.ReadAllText(path), SerializerOptions)
            ?? throw new InvalidOperationException("Nie mozna wczytac szablonu: " + path);
        dto.Validate(path);
        return LibraryTemplate.FromDto(dto);
    }
}
