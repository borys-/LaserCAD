using LaserCad.Geometry;

namespace LaserCad.Core.Documents;

/// <summary>
/// Bazowy kontrakt encji szkicu 2D.
/// Uzywaj go, gdy kod potrzebuje wspolnie obslugiwac rozne typy encji bez znajomosci ich geometrii szczegolowej.
/// </summary>
public interface ISketchEntity
{
    /// <summary>
    /// Stabilny identyfikator encji w szkicu.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Nazwa warstwy, do ktorej nalezy encja.
    /// W MVP nazwa warstwy pelni role identyfikatora warstwy.
    /// </summary>
    string LayerName { get; }

    /// <summary>
    /// Bounding box encji w milimetrach domenowych.
    /// </summary>
    BoundingBox Bounds { get; }

    /// <summary>
    /// Zwraca nowa encje po zastosowaniu transformacji afinicznej.
    /// </summary>
    ISketchEntity Transform(Matrix3x3 transform);
}
