using LaserCad.Core.Preview3D;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Wynik przyciagniecia plyty materialowej do powierzchni innej plyty.
/// </summary>
public sealed class MaterialSolidSurfaceAttachment
{
    /// <summary>
    /// Tworzy opis relacji powierzchniowej miedzy plytami.
    /// </summary>
    public MaterialSolidSurfaceAttachment(
        Guid targetMaterialSolidId,
        MaterialSolidOrientation orientation,
        Point3D anchorPoint)
    {
        if (targetMaterialSolidId == Guid.Empty)
        {
            throw new ArgumentException("Target material solid id cannot be empty.", nameof(targetMaterialSolidId));
        }

        TargetMaterialSolidId = targetMaterialSolidId;
        Orientation = orientation ?? throw new ArgumentNullException(nameof(orientation));
        AnchorPoint = anchorPoint;
    }

    /// <summary>
    /// Plyta, do ktorej przyciagnieto nowy element.
    /// </summary>
    public Guid TargetMaterialSolidId { get; }

    /// <summary>
    /// Orientacja, ktora nalezy nadac przyciaganej plycie.
    /// </summary>
    public MaterialSolidOrientation Orientation { get; }

    /// <summary>
    /// Punkt zaczepienia na powierzchni docelowej.
    /// </summary>
    public Point3D AnchorPoint { get; }
}
