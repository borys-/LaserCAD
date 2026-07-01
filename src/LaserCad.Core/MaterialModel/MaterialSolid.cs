using LaserCad.Core.Documents;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Domena elementu materialowego 3D, ktorego grubosc wynika z profilu materialu.
/// </summary>
public sealed class MaterialSolid
{
    /// <summary>
    /// Tworzy element materialowy powiazany z profilem materialu.
    /// </summary>
    public MaterialSolid(Guid id, string name, MaterialProfile materialProfile)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Material solid id cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Material solid name cannot be empty.", nameof(name));
        }

        Id = id;
        Name = name;
        MaterialProfile = materialProfile ?? throw new ArgumentNullException(nameof(materialProfile));
    }

    /// <summary>
    /// Tworzy element materialowy z nowym identyfikatorem.
    /// </summary>
    public MaterialSolid(string name, MaterialProfile materialProfile)
        : this(Guid.NewGuid(), name, materialProfile)
    {
    }

    /// <summary>
    /// Stabilny identyfikator elementu materialowego.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Nazwa elementu materialowego widoczna w modelu.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Profil materialu, z ktorego pochodzi grubosc elementu.
    /// </summary>
    public MaterialProfile MaterialProfile { get; }

    /// <summary>
    /// Grubosc elementu materialowego wynikajaca bezposrednio z profilu materialu.
    /// </summary>
    public Length Thickness => MaterialProfile.Thickness;
}
