using LaserCad.Core.Preview3D;
using LaserCad.Geometry;

namespace LaserCad.Core.MaterialModel;

/// <summary>
/// Wykrywa proste kolizje AABB miedzy plytami materialowymi.
/// </summary>
public sealed class MaterialSolidCollisionDetector
{
    /// <summary>
    /// Zwraca pary plyt, ktorych prostopadloscienne zakresy nachodza na siebie.
    /// </summary>
    public IReadOnlyList<MaterialSolidCollision> FindCollisions(IEnumerable<MaterialSolid> materialSolids)
    {
        if (materialSolids is null)
        {
            throw new ArgumentNullException(nameof(materialSolids));
        }

        var solids = materialSolids.ToArray();
        var collisions = new List<MaterialSolidCollision>();

        for (var firstIndex = 0; firstIndex < solids.Length; firstIndex++)
        {
            for (var secondIndex = firstIndex + 1; secondIndex < solids.Length; secondIndex++)
            {
                if (Intersects(GetBounds(solids[firstIndex]), GetBounds(solids[secondIndex])))
                {
                    collisions.Add(new MaterialSolidCollision(solids[firstIndex].Id, solids[secondIndex].Id));
                }
            }
        }

        return collisions;
    }

    private static (Point3D Min, Point3D Max) GetBounds(MaterialSolid solid)
    {
        var offset = solid.Orientation.Position;
        var vertices = solid.Mesh.Vertices
            .Select(vertex => new Point3D(vertex.X + offset.X, vertex.Y + offset.Y, vertex.Z + offset.Z))
            .ToArray();

        return (
            new Point3D(vertices.Min(vertex => vertex.X), vertices.Min(vertex => vertex.Y), vertices.Min(vertex => vertex.Z)),
            new Point3D(vertices.Max(vertex => vertex.X), vertices.Max(vertex => vertex.Y), vertices.Max(vertex => vertex.Z)));
    }

    private static bool Intersects((Point3D Min, Point3D Max) first, (Point3D Min, Point3D Max) second)
    {
        return first.Min.X < second.Max.X - GeometryTolerance.Default
            && first.Max.X > second.Min.X + GeometryTolerance.Default
            && first.Min.Y < second.Max.Y - GeometryTolerance.Default
            && first.Max.Y > second.Min.Y + GeometryTolerance.Default
            && first.Min.Z < second.Max.Z - GeometryTolerance.Default
            && first.Max.Z > second.Min.Z + GeometryTolerance.Default;
    }
}
