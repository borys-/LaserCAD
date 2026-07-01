namespace LaserCad.Core.Preview3D;

/// <summary>
/// Wektor 3D w milimetrach uzywany do orientacji elementow materialowych.
/// </summary>
public readonly record struct Vector3D(double X, double Y, double Z)
{
    /// <summary>
    /// Wektor jednostkowy wzdluz osi X.
    /// </summary>
    public static Vector3D UnitX { get; } = new(1.0, 0.0, 0.0);

    /// <summary>
    /// Wektor normalny skierowany w gore wzdluz osi Z.
    /// </summary>
    public static Vector3D UnitZ { get; } = new(0.0, 0.0, 1.0);

    /// <summary>
    /// Dlugosc wektora.
    /// </summary>
    public double Length => Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
}
