namespace LaserCad.Geometry;

/// <summary>
/// Prosty polygon 2D opisany lista wierzcholkow.
/// Uzywaj go dla zamknietych konturow powierzchniowych.
/// </summary>
public sealed class Polygon2D
{
    private readonly Point2D[] vertices;
    private readonly IReadOnlyList<Point2D> readOnlyVertices;

    /// <summary>
    /// Tworzy polygon z podanej listy wierzcholkow.
    /// </summary>
    public Polygon2D(IEnumerable<Point2D> vertices)
    {
        if (vertices is null)
        {
            throw new ArgumentNullException(nameof(vertices));
        }

        this.vertices = vertices.ToArray();

        if (this.vertices.Length < 3)
        {
            throw new ArgumentException("Polygon must contain at least three vertices.", nameof(vertices));
        }

        if (Math.Abs(SignedArea) <= GeometryTolerance.Default)
        {
            throw new ArgumentException("Polygon area must be greater than zero.", nameof(vertices));
        }

        readOnlyVertices = Array.AsReadOnly(this.vertices);
    }

    /// <summary>
    /// Wierzcholki polygonu w kolejnosci obiegu.
    /// </summary>
    public IReadOnlyList<Point2D> Vertices => readOnlyVertices;

    /// <summary>
    /// Pole powierzchni polygonu w kwadratowych milimetrach domenowych.
    /// </summary>
    public double Area => Math.Abs(SignedArea);

    /// <summary>
    /// Orientacja wierzcholkow polygonu.
    /// </summary>
    public PolygonOrientation Orientation => SignedArea < 0.0
        ? PolygonOrientation.Clockwise
        : PolygonOrientation.Counterclockwise;

    /// <summary>
    /// Bounding box obejmujacy wszystkie wierzcholki polygonu.
    /// </summary>
    public BoundingBox Bounds => BoundingBox.FromPoints(vertices);

    private double SignedArea => CalculateSignedArea(vertices);

    /// <summary>
    /// Zwraca polygon po zastosowaniu transformacji afinicznej do wszystkich wierzcholkow.
    /// </summary>
    public Polygon2D Transform(Matrix3x3 transform)
    {
        return new Polygon2D(vertices.Select(transform.Transform));
    }

    private static double CalculateSignedArea(IReadOnlyList<Point2D> vertices)
    {
        double doubleArea = 0.0;

        for (int i = 0; i < vertices.Count; i++)
        {
            Point2D current = vertices[i];
            Point2D next = vertices[(i + 1) % vertices.Count];
            doubleArea += (current.X * next.Y) - (next.X * current.Y);
        }

        return doubleArea / 2.0;
    }
}
