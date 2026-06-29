namespace LaserCad.Core.Documents;

/// <summary>
/// Serializuje i deserializuje dokument CAD do stabilnego formatu pliku projektu.
/// Obecnie obslugiwanym formatem jest JSON w wersji 1.
/// </summary>
public sealed class DocumentSerializer
{
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

        throw new NotImplementedException("Document serialization is not implemented yet.");
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
}
