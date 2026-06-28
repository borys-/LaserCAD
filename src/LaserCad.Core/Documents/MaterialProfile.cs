using LaserCad.Geometry.Units;

namespace LaserCad.Core.Documents;

/// <summary>
/// Reprezentuje profil materialu przypisany do dokumentu.
/// Przechowuje nazwe oraz parametry produkcyjne uzywane przez generatory i kompensacje ciecia.
/// </summary>
public sealed class MaterialProfile
{
    /// <summary>
    /// Tworzy profil materialu o podanej nazwie.
    /// Uzywaj nazw opisowych, np. "Plywood 3 mm" albo "Acrylic".
    /// </summary>
    public MaterialProfile(string name, Length? thickness = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Material profile name cannot be empty.", nameof(name));
        }

        Thickness = thickness ?? Length.FromMillimeters(0.0);
        if (Thickness < Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(nameof(thickness), "Material thickness cannot be negative.");
        }

        Name = name;
    }

    /// <summary>
    /// Nazwa materialu prezentowana uzytkownikowi.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Grubosc materialu.
    /// </summary>
    public Length Thickness { get; }
}
