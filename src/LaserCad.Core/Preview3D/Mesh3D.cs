using LaserCad.Geometry;

namespace LaserCad.Core.Preview3D;

/// <summary>
/// Prosty mesh 3D uzywany przez podglad zlozonych elementow.
/// Wspolrzedne X/Y sa w milimetrach dokumentu, a Z oznacza grubosc materialu.
/// </summary>
public sealed class Mesh3D
{
    private readonly Point3D[] vertices;
    private readonly int[] triangleIndices;

    /// <summary>
    /// Tworzy mesh z wierzcholkow i indeksow trojkatow.
    /// </summary>
    public Mesh3D(IEnumerable<Point3D> vertices, IEnumerable<int> triangleIndices)
    {
        if (vertices is null)
        {
            throw new ArgumentNullException(nameof(vertices));
        }

        if (triangleIndices is null)
        {
            throw new ArgumentNullException(nameof(triangleIndices));
        }

        this.vertices = vertices.ToArray();
        this.triangleIndices = triangleIndices.ToArray();

        if (this.vertices.Length == 0)
        {
            throw new ArgumentException("Mesh must contain at least one vertex.", nameof(vertices));
        }

        if (this.triangleIndices.Length == 0 || this.triangleIndices.Length % 3 != 0)
        {
            throw new ArgumentException("Triangle index count must be positive and divisible by three.", nameof(triangleIndices));
        }

        foreach (var index in this.triangleIndices)
        {
            if (index < 0 || index >= this.vertices.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(triangleIndices), "Triangle index is outside vertex range.");
            }
        }
    }

    /// <summary>
    /// Wierzcholki mesha.
    /// </summary>
    public IReadOnlyList<Point3D> Vertices => vertices;

    /// <summary>
    /// Indeksy trojkatow, po trzy indeksy na trojkat.
    /// </summary>
    public IReadOnlyList<int> TriangleIndices => triangleIndices;

    /// <summary>
    /// Liczba trojkatow w meshu.
    /// </summary>
    public int TriangleCount => triangleIndices.Length / 3;

    /// <summary>
    /// Bounding box rzutu mesha na plaszczyzne XY.
    /// </summary>
    public BoundingBox Bounds2D => BoundingBox.FromPoints(vertices.Select(vertex => new Point2D(vertex.X, vertex.Y)).ToArray());
}
