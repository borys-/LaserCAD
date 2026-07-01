using LaserCad.Core.Preview3D;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Lekki opis relacji montazowej pokazywany przed zatwierdzeniem polaczenia plyt.
/// </summary>
public sealed class MaterialSolidConnectionPreview
{
    /// <summary>
    /// Tworzy podglad relacji miedzy dwiema plytami.
    /// </summary>
    public MaterialSolidConnectionPreview(
        Guid sourceMaterialSolidId,
        Guid targetMaterialSolidId,
        Point3D anchorPoint,
        double angleRadians)
    {
        if (sourceMaterialSolidId == Guid.Empty)
        {
            throw new ArgumentException("Source material solid id cannot be empty.", nameof(sourceMaterialSolidId));
        }

        if (targetMaterialSolidId == Guid.Empty)
        {
            throw new ArgumentException("Target material solid id cannot be empty.", nameof(targetMaterialSolidId));
        }

        SourceMaterialSolidId = sourceMaterialSolidId;
        TargetMaterialSolidId = targetMaterialSolidId;
        AnchorPoint = anchorPoint;
        AngleRadians = angleRadians;
    }

    /// <summary>
    /// Plyta dolaczana.
    /// </summary>
    public Guid SourceMaterialSolidId { get; }

    /// <summary>
    /// Plyta docelowa.
    /// </summary>
    public Guid TargetMaterialSolidId { get; }

    /// <summary>
    /// Punkt zaczepienia relacji.
    /// </summary>
    public Point3D AnchorPoint { get; }

    /// <summary>
    /// Kat relacji montazowej.
    /// </summary>
    public double AngleRadians { get; }
}
