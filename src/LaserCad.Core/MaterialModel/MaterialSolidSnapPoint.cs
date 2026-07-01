using LaserCad.Core.Preview3D;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Wynik snapowania do elementu materialowego 3D.
/// </summary>
public sealed class MaterialSolidSnapPoint
{
    /// <summary>
    /// Tworzy punkt snapowania dla konkretnej plyty.
    /// </summary>
    public MaterialSolidSnapPoint(Guid materialSolidId, Point3D position, MaterialSolidSnapKind kind, double distance)
    {
        if (materialSolidId == Guid.Empty)
        {
            throw new ArgumentException("Material solid id cannot be empty.", nameof(materialSolidId));
        }

        if (distance < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(distance), "Snap distance cannot be negative.");
        }

        MaterialSolidId = materialSolidId;
        Position = position;
        Kind = kind;
        Distance = distance;
    }

    /// <summary>
    /// Identyfikator plyty, do ktorej znaleziono snap.
    /// </summary>
    public Guid MaterialSolidId { get; }

    /// <summary>
    /// Pozycja snapowania w przestrzeni modelu.
    /// </summary>
    public Point3D Position { get; }

    /// <summary>
    /// Rodzaj snapu: naroznik albo krawedz.
    /// </summary>
    public MaterialSolidSnapKind Kind { get; }

    /// <summary>
    /// Odleglosc od punktu zadanego przez uzytkownika.
    /// </summary>
    public double Distance { get; }
}
