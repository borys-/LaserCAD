using System.Globalization;

namespace LaserCad.Core.Documents;

/// <summary>
/// Reprezentuje kolor warstwy zapisany jako szesnastkowe RGB w formacie #RRGGBB.
/// Format jest dobrany tak, aby byl bezposrednio uzyteczny przy eksporcie SVG.
/// </summary>
public sealed record LayerColor
{
    /// <summary>
    /// Tworzy kolor warstwy z tekstu w formacie #RRGGBB.
    /// </summary>
    public LayerColor(string hex)
    {
        if (!IsValidHex(hex))
        {
            throw new ArgumentException("Layer color must use #RRGGBB format.", nameof(hex));
        }

        Hex = hex.ToUpperInvariant();
    }

    /// <summary>
    /// Domyslny czarny kolor warstwy.
    /// </summary>
    public static LayerColor Black { get; } = new("#000000");

    /// <summary>
    /// Szesnastkowy zapis koloru w formacie #RRGGBB.
    /// </summary>
    public string Hex { get; }

    /// <summary>
    /// Tworzy kolor warstwy z tekstu w formacie #RRGGBB.
    /// </summary>
    public static LayerColor FromHex(string hex)
    {
        return new LayerColor(hex);
    }

    /// <summary>
    /// Zwraca tekstowy zapis koloru.
    /// </summary>
    public override string ToString()
    {
        return Hex;
    }

    /// <summary>
    /// Sprawdza, czy tekst uzywa formatu #RRGGBB.
    /// </summary>
    private static bool IsValidHex(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex) || hex.Length != 7 || hex[0] != '#')
        {
            return false;
        }

        return int.TryParse(hex[1..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _);
    }
}
