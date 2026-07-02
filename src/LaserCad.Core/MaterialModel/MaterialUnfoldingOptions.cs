namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Opcje rozwijania elementow materialowych 3D do plaskich czesci produkcyjnych.
/// </summary>
public sealed class MaterialUnfoldingOptions
{
    /// <summary>
    /// Tworzy opcje rozwijania.
    /// </summary>
    public MaterialUnfoldingOptions(bool mergeIdenticalParts = false)
    {
        MergeIdenticalParts = mergeIdenticalParts;
    }

    /// <summary>
    /// Okresla, czy identyczne geometrycznie czesci maja byc scalane przez zwiekszenie pola Quantity.
    /// </summary>
    public bool MergeIdenticalParts { get; }
}
