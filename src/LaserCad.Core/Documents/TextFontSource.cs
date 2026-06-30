namespace LaserCad.Core.Documents;

/// <summary>
/// Informacja o zrodle fontu uzywanego przez encje tekstowa.
/// </summary>
public sealed class TextFontSource
{
    /// <summary>
    /// Tworzy opis zrodla fontu.
    /// </summary>
    public TextFontSource(string familyName, string? filePath = null)
    {
        if (string.IsNullOrWhiteSpace(familyName))
        {
            throw new ArgumentException("Font family name cannot be empty.", nameof(familyName));
        }

        FamilyName = familyName;
        FilePath = string.IsNullOrWhiteSpace(filePath) ? null : filePath;
    }

    /// <summary>
    /// Domyslny font tekstu MVP.
    /// </summary>
    public static TextFontSource Default { get; } = new("Arial");

    /// <summary>
    /// Nazwa rodziny fontu.
    /// </summary>
    public string FamilyName { get; }

    /// <summary>
    /// Opcjonalna sciezka do zaimportowanego pliku fontu.
    /// </summary>
    public string? FilePath { get; }

    /// <summary>
    /// Zwraca kopie z podmieniona nazwa rodziny fontu.
    /// </summary>
    public TextFontSource WithFamilyName(string familyName)
    {
        return new TextFontSource(familyName, FilePath);
    }

    /// <summary>
    /// Zwraca kopie z podmieniona sciezka pliku fontu.
    /// </summary>
    public TextFontSource WithFilePath(string? filePath)
    {
        return new TextFontSource(FamilyName, filePath);
    }
}
