namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Opis kolizji dwoch plyt materialowych.
/// </summary>
public sealed class MaterialSolidCollision
{
    /// <summary>
    /// Tworzy wynik kolizji dla pary plyt.
    /// </summary>
    public MaterialSolidCollision(Guid firstMaterialSolidId, Guid secondMaterialSolidId)
    {
        if (firstMaterialSolidId == Guid.Empty)
        {
            throw new ArgumentException("First material solid id cannot be empty.", nameof(firstMaterialSolidId));
        }

        if (secondMaterialSolidId == Guid.Empty)
        {
            throw new ArgumentException("Second material solid id cannot be empty.", nameof(secondMaterialSolidId));
        }

        FirstMaterialSolidId = firstMaterialSolidId;
        SecondMaterialSolidId = secondMaterialSolidId;
    }

    /// <summary>
    /// Pierwsza plyta w kolizji.
    /// </summary>
    public Guid FirstMaterialSolidId { get; }

    /// <summary>
    /// Druga plyta w kolizji.
    /// </summary>
    public Guid SecondMaterialSolidId { get; }
}
