using LaserCad.Core.Documents;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.Library;

/// <summary>
/// Profil materialu wczytany z biblioteki aplikacji.
/// </summary>
public sealed class LibraryMaterialProfile
{
    /// <summary>
    /// Tworzy opis profilu materialu biblioteki.
    /// </summary>
    public LibraryMaterialProfile(string id, MaterialProfile profile)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Identyfikator profilu materialu nie moze byc pusty.", nameof(id));
        }

        Id = id;
        Profile = profile ?? throw new ArgumentNullException(nameof(profile));
    }

    /// <summary>
    /// Stabilny identyfikator profilu w bibliotece.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Domenowy profil materialu.
    /// </summary>
    public MaterialProfile Profile { get; }

    internal static LibraryMaterialProfile FromDto(MaterialProfileDto dto)
    {
        return new LibraryMaterialProfile(
            dto.Id,
            new MaterialProfile(
                dto.Name,
                Length.FromMillimeters(dto.ThicknessMillimeters),
                Length.FromMillimeters(dto.DefaultKerfMillimeters),
                Length.FromMillimeters(dto.DefaultClearanceMillimeters),
                Length.FromMillimeters(dto.MinimumFingerWidthMillimeters)));
    }
}
