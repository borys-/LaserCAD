using LaserCad.Geometry.Units;

namespace LaserCad.Core.Documents;

/// <summary>
/// Katalog standardowych profili materialow dla typowych prac laserowych.
/// Wartosci sa punktami startowymi i moga byc pozniej korygowane pod konkretna maszyne.
/// </summary>
public static class DefaultMaterialProfiles
{
    /// <summary>
    /// Profil sklejki o grubosci 3 mm.
    /// </summary>
    public static MaterialProfile Plywood3Mm { get; } = new(
        "Sklejka 3 mm",
        thickness: Length.FromMillimeters(3.0),
        defaultKerf: Length.FromMillimeters(0.15),
        defaultClearance: Length.FromMillimeters(0.1),
        minimumFingerWidth: Length.FromMillimeters(3.0));

    /// <summary>
    /// Profil sklejki o grubosci 4 mm.
    /// </summary>
    public static MaterialProfile Plywood4Mm { get; } = new(
        "Sklejka 4 mm",
        thickness: Length.FromMillimeters(4.0),
        defaultKerf: Length.FromMillimeters(0.16),
        defaultClearance: Length.FromMillimeters(0.1),
        minimumFingerWidth: Length.FromMillimeters(4.0));

    /// <summary>
    /// Profil plyty MDF o grubosci 3 mm.
    /// </summary>
    public static MaterialProfile Mdf { get; } = new(
        "MDF 3 mm",
        thickness: Length.FromMillimeters(3.0),
        defaultKerf: Length.FromMillimeters(0.18),
        defaultClearance: Length.FromMillimeters(0.1),
        minimumFingerWidth: Length.FromMillimeters(3.0));

    /// <summary>
    /// Profil akrylu o grubosci 3 mm.
    /// </summary>
    public static MaterialProfile Acrylic { get; } = new(
        "Akryl 3 mm",
        thickness: Length.FromMillimeters(3.0),
        defaultKerf: Length.FromMillimeters(0.14),
        defaultClearance: Length.FromMillimeters(0.08),
        minimumFingerWidth: Length.FromMillimeters(3.0));

    /// <summary>
    /// Pelna lista domyslnych profili materialow w kolejnosci prezentacji.
    /// </summary>
    public static IReadOnlyList<MaterialProfile> All { get; } = new[] { Plywood3Mm, Plywood4Mm, Mdf, Acrylic };
}
