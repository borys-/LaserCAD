using LaserCad.Core.Preview3D;
using LaserCad.Geometry;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Wyszukuje snap do narozy i krawedzi istniejacych plyt materialowych.
/// </summary>
public sealed class MaterialSolidSnapService
{
    /// <summary>
    /// Znajduje najblizszy snap w zadanej tolerancji.
    /// </summary>
    public MaterialSolidSnapPoint? FindNearest(
        IEnumerable<MaterialSolid> materialSolids,
        Point3D point,
        double toleranceMillimeters)
    {
        if (materialSolids is null)
        {
            throw new ArgumentNullException(nameof(materialSolids));
        }

        if (toleranceMillimeters < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(toleranceMillimeters), "Snap tolerance cannot be negative.");
        }

        MaterialSolidSnapPoint? nearestCorner = null;
        MaterialSolidSnapPoint? nearestEdge = null;

        foreach (var materialSolid in materialSolids)
        {
            foreach (var candidate in EnumerateCandidates(materialSolid, point))
            {
                if (candidate.Distance > toleranceMillimeters)
                {
                    continue;
                }

                if (candidate.Kind == MaterialSolidSnapKind.Corner)
                {
                    if (nearestCorner is null || candidate.Distance < nearestCorner.Distance)
                    {
                        nearestCorner = candidate;
                    }

                    continue;
                }

                if (nearestEdge is null || candidate.Distance < nearestEdge.Distance)
                {
                    nearestEdge = candidate;
                }
            }
        }

        return nearestCorner ?? nearestEdge;
    }

    private static IEnumerable<MaterialSolidSnapPoint> EnumerateCandidates(MaterialSolid materialSolid, Point3D point)
    {
        var corners = GetFootprintCorners(materialSolid);
        foreach (var corner in corners)
        {
            yield return new MaterialSolidSnapPoint(
                materialSolid.Id,
                corner,
                MaterialSolidSnapKind.Corner,
                Distance(point, corner));
        }

        for (var index = 0; index < corners.Length; index++)
        {
            var start = corners[index];
            var end = corners[(index + 1) % corners.Length];
            var projected = ProjectToSegment(point, start, end);

            yield return new MaterialSolidSnapPoint(
                materialSolid.Id,
                projected,
                MaterialSolidSnapKind.Edge,
                Distance(point, projected));
        }
    }

    private static Point3D[] GetFootprintCorners(MaterialSolid materialSolid)
    {
        var bounds = materialSolid.Mesh.Bounds2D;
        var offset = materialSolid.Orientation.Position;

        return new[]
        {
            new Point3D(bounds.MinX + offset.X, bounds.MinY + offset.Y, offset.Z),
            new Point3D(bounds.MaxX + offset.X, bounds.MinY + offset.Y, offset.Z),
            new Point3D(bounds.MaxX + offset.X, bounds.MaxY + offset.Y, offset.Z),
            new Point3D(bounds.MinX + offset.X, bounds.MaxY + offset.Y, offset.Z),
        };
    }

    private static Point3D ProjectToSegment(Point3D point, Point3D start, Point3D end)
    {
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        var dz = end.Z - start.Z;
        var lengthSquared = (dx * dx) + (dy * dy) + (dz * dz);
        if (lengthSquared <= GeometryTolerance.Default)
        {
            return start;
        }

        var t = (((point.X - start.X) * dx) + ((point.Y - start.Y) * dy) + ((point.Z - start.Z) * dz)) / lengthSquared;
        t = Math.Max(0.0, Math.Min(1.0, t));

        return new Point3D(
            start.X + (dx * t),
            start.Y + (dy * t),
            start.Z + (dz * t));
    }

    private static double Distance(Point3D a, Point3D b)
    {
        var dx = a.X - b.X;
        var dy = a.Y - b.Y;
        var dz = a.Z - b.Z;
        return Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz));
    }
}
