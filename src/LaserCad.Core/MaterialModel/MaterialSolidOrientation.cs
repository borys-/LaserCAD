using LaserCad.Core.Preview3D;
using LaserCad.Geometry;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Orientacja elementu materialowego w przestrzeni 3D.
/// </summary>
public sealed class MaterialSolidOrientation
{
    /// <summary>
    /// Domyslna orientacja elementu lezacego na plaszczyznie XY.
    /// </summary>
    public static MaterialSolidOrientation Default { get; } = new(
        new Point3D(0.0, 0.0, 0.0),
        0.0,
        Vector3D.UnitZ);

    /// <summary>
    /// Tworzy orientacje z pozycji, obrotu i normalnej powierzchni.
    /// </summary>
    public MaterialSolidOrientation(Point3D position, double rotationRadians, Vector3D surfaceNormal)
    {
        if (surfaceNormal.Length <= GeometryTolerance.Default)
        {
            throw new ArgumentException("Surface normal cannot be zero.", nameof(surfaceNormal));
        }

        Position = position;
        RotationRadians = rotationRadians;
        SurfaceNormal = surfaceNormal;
    }

    /// <summary>
    /// Pozycja lokalnego poczatku elementu w przestrzeni 3D.
    /// </summary>
    public Point3D Position { get; }

    /// <summary>
    /// Obrot elementu wokol normalnej powierzchni w radianach.
    /// </summary>
    public double RotationRadians { get; }

    /// <summary>
    /// Normalna powierzchni elementu materialowego.
    /// </summary>
    public Vector3D SurfaceNormal { get; }
}
