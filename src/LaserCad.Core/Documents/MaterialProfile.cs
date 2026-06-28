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
    public MaterialProfile(
        string name,
        Length? thickness = null,
        Length? defaultKerf = null,
        Length? defaultClearance = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Material profile name cannot be empty.", nameof(name));
        }

        Thickness = EnsureNonNegative(thickness ?? Length.FromMillimeters(0.0), nameof(thickness), "Material thickness cannot be negative.");
        DefaultKerf = EnsureNonNegative(defaultKerf ?? Length.FromMillimeters(0.0), nameof(defaultKerf), "Default kerf cannot be negative.");
        DefaultClearance = EnsureNonNegative(defaultClearance ?? Length.FromMillimeters(0.0), nameof(defaultClearance), "Default clearance cannot be negative.");

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

    /// <summary>
    /// Domyslna szerokosc szczeliny ciecia dla materialu.
    /// </summary>
    public Length DefaultKerf { get; }

    /// <summary>
    /// Domyslny luz montazowy dla polaczen generowanych z profilu materialu.
    /// </summary>
    public Length DefaultClearance { get; }

    /// <summary>
    /// Zwraca dlugosc po sprawdzeniu, ze nie jest ujemna.
    /// </summary>
    private static Length EnsureNonNegative(Length value, string parameterName, string message)
    {
        if (value < Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(parameterName, message);
        }

        return value;
    }
}
