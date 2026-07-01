using LaserCad.Core.Preview3D;
using LaserCad.Geometry;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Operacje montazowe dla plyt materialowych 3D.
/// </summary>
public sealed class MaterialSolidAssemblyService
{
    /// <summary>
    /// Przyciaga plyte do najblizszego punktu powierzchni docelowej, zachowujac jej normalna.
    /// </summary>
    public MaterialSolidSurfaceAttachment AttachToSurface(
        MaterialSolid movingSolid,
        MaterialSolid targetSolid,
        Point3D requestedPoint)
    {
        if (movingSolid is null)
        {
            throw new ArgumentNullException(nameof(movingSolid));
        }

        if (targetSolid is null)
        {
            throw new ArgumentNullException(nameof(targetSolid));
        }

        var targetBounds = GetWorldBounds(targetSolid);
        var clampedPoint = new Point3D(
            Clamp(requestedPoint.X, targetBounds.Min.X, targetBounds.Max.X),
            Clamp(requestedPoint.Y, targetBounds.Min.Y, targetBounds.Max.Y),
            targetBounds.Max.Z);

        return new MaterialSolidSurfaceAttachment(
            targetSolid.Id,
            new MaterialSolidOrientation(
                clampedPoint,
                movingSolid.Orientation.RotationRadians,
                targetSolid.Orientation.SurfaceNormal),
            clampedPoint);
    }

    /// <summary>
    /// Tworzy orientacje laczenia plyty z celem pod katem 90 stopni.
    /// </summary>
    public MaterialSolidOrientation CreateRightAngleJoint(
        MaterialSolid movingSolid,
        MaterialSolid targetSolid,
        Point3D anchorPoint)
    {
        if (movingSolid is null)
        {
            throw new ArgumentNullException(nameof(movingSolid));
        }

        if (targetSolid is null)
        {
            throw new ArgumentNullException(nameof(targetSolid));
        }

        return new MaterialSolidOrientation(
            anchorPoint,
            targetSolid.Orientation.RotationRadians + (Math.PI / 2.0),
            RotateNormalByRightAngle(targetSolid.Orientation.SurfaceNormal));
    }

    /// <summary>
    /// Tworzy podglad relacji montazowej pod katem prostym.
    /// </summary>
    public MaterialSolidConnectionPreview CreateRightAnglePreview(
        MaterialSolid movingSolid,
        MaterialSolid targetSolid,
        Point3D anchorPoint)
    {
        if (movingSolid is null)
        {
            throw new ArgumentNullException(nameof(movingSolid));
        }

        if (targetSolid is null)
        {
            throw new ArgumentNullException(nameof(targetSolid));
        }

        return new MaterialSolidConnectionPreview(
            movingSolid.Id,
            targetSolid.Id,
            anchorPoint,
            Math.PI / 2.0);
    }

    private static (Point3D Min, Point3D Max) GetWorldBounds(MaterialSolid solid)
    {
        var offset = solid.Orientation.Position;
        var vertices = solid.Mesh.Vertices
            .Select(vertex => new Point3D(vertex.X + offset.X, vertex.Y + offset.Y, vertex.Z + offset.Z))
            .ToArray();

        return (
            new Point3D(vertices.Min(vertex => vertex.X), vertices.Min(vertex => vertex.Y), vertices.Min(vertex => vertex.Z)),
            new Point3D(vertices.Max(vertex => vertex.X), vertices.Max(vertex => vertex.Y), vertices.Max(vertex => vertex.Z)));
    }

    private static Vector3D RotateNormalByRightAngle(Vector3D normal)
    {
        if (Math.Abs(normal.X) <= GeometryTolerance.Default
            && Math.Abs(normal.Y) <= GeometryTolerance.Default)
        {
            return Vector3D.UnitX;
        }

        return new Vector3D(-normal.Y, normal.X, normal.Z);
    }

    private static double Clamp(double value, double min, double max)
    {
        return Math.Max(min, Math.Min(max, value));
    }
}
