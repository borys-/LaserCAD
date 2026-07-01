using System.Text.Json;

namespace LaserCad.Core.Library;

internal sealed class TemplateDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string GeneratorType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public Dictionary<string, object> Parameters { get; set; } = new();

    public void Validate(string path)
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            throw new InvalidOperationException("Szablon nie ma identyfikatora: " + path);
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Szablon nie ma nazwy: " + path);
        }

        if (string.IsNullOrWhiteSpace(GeneratorType))
        {
            throw new InvalidOperationException("Szablon nie ma typu generatora: " + path);
        }

        Parameters = Parameters.ToDictionary(pair => pair.Key, pair => ConvertValue(pair.Value), StringComparer.OrdinalIgnoreCase);
    }

    private static object ConvertValue(object value)
    {
        if (value is not JsonElement element)
        {
            return value;
        }

        return element.ValueKind switch
        {
            JsonValueKind.Number when element.TryGetInt32(out var integer) => integer,
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => element.ToString(),
        };
    }
}
