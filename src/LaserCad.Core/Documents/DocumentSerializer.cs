using System.Text.Json;
using System.Text.Json.Serialization;

namespace LaserCad.Core.Documents;

/// <summary>
/// Serializuje i deserializuje dokument CAD do stabilnego formatu pliku projektu.
/// Obecnie obslugiwanym formatem jest JSON w wersji 1.
/// </summary>
public sealed class DocumentSerializer
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <summary>
    /// Biezaca wersja formatu pliku obslugiwana przez serializer.
    /// </summary>
    public const int SupportedFormatVersion = 1;

    /// <summary>
    /// Serializuje dokument CAD do tekstu JSON.
    /// </summary>
    public string Serialize(CadDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);

        var dto = new DocumentDto
        {
            Id = document.Id,
            Name = document.Name
        };

        return JsonSerializer.Serialize(dto, JsonOptions);
    }

    /// <summary>
    /// Odtwarza dokument CAD z tekstu JSON.
    /// </summary>
    public CadDocument Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentException("Document JSON cannot be empty.", nameof(json));
        }

        throw new NotImplementedException("Document deserialization is not implemented yet.");
    }

    private sealed class DocumentDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
