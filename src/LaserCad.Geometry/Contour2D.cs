namespace LaserCad.Geometry;

/// <summary>
/// Kontur 2D opisany uporzadkowana lista punktow.
/// Uzywaj go do walidacji zamknietych obrysow i operacji przygotowujacych offset.
/// </summary>
public sealed class Contour2D
{
    private readonly Point2D[] points;
    private readonly IReadOnlyList<Point2D> readOnlyPoints;

    /// <summary>
    /// Tworzy kontur z podanej listy punktow.
    /// Kontur jest uznany za domkniety, gdy pierwszy i ostatni punkt sa rowne w tolerancji geometrycznej.
    /// </summary>
    public Contour2D(IEnumerable<Point2D> points)
    {
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        this.points = points.ToArray();

        if (this.points.Length < 2)
        {
            throw new ArgumentException("Contour must contain at least two points.", nameof(points));
        }

        IsClosed = AreSamePoint(this.points[0], this.points[^1]);
        readOnlyPoints = Array.AsReadOnly(this.points);
    }

    private Contour2D(IEnumerable<Point2D> points, bool isClosed)
    {
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        this.points = points.ToArray();

        if (this.points.Length < 2)
        {
            throw new ArgumentException("Contour must contain at least two points.", nameof(points));
        }

        IsClosed = isClosed;
        readOnlyPoints = Array.AsReadOnly(this.points);
    }

    /// <summary>
    /// Punkty konturu w kolejnosci obiegu.
    /// </summary>
    public IReadOnlyList<Point2D> Points => readOnlyPoints;

    /// <summary>
    /// Informuje, czy kontur jest domkniety.
    /// </summary>
    public bool IsClosed { get; }

    /// <summary>
    /// Bounding box obejmujacy punkty konturu.
    /// </summary>
    public BoundingBox Bounds => BoundingBox.FromPoints(points);

    /// <summary>
    /// Tworzy domkniety kontur z polygonu bez powielania pierwszego punktu na koncu listy.
    /// </summary>
    public static Contour2D FromPolygon(Polygon2D polygon)
    {
        if (polygon is null)
        {
            throw new ArgumentNullException(nameof(polygon));
        }

        return new Contour2D(polygon.Vertices, isClosed: true);
    }

    /// <summary>
    /// Sprawdza, czy zamkniety kontur ma samoprzeciecia miedzy niesasiadujacymi krawedziami.
    /// </summary>
    public bool HasSelfIntersections()
    {
        if (!IsClosed || points.Length < 4)
        {
            return false;
        }

        LineSegment2D[] segments = GetSegments().ToArray();

        for (int i = 0; i < segments.Length; i++)
        {
            for (int j = i + 1; j < segments.Length; j++)
            {
                if (AreAdjacentSegments(i, j, segments.Length))
                {
                    continue;
                }

                IntersectionResult intersection = Intersections2D.Intersect(segments[i], segments[j]);

                if (intersection.IsPoint || intersection.IsOverlap)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Zwraca odcinki skladajace sie na kontur.
    /// Dla konturu domknietego ostatni odcinek laczy ostatni unikalny punkt z pierwszym.
    /// </summary>
    public IEnumerable<LineSegment2D> GetSegments()
    {
        int segmentPointCount = IsClosed && AreSamePoint(points[0], points[^1])
            ? points.Length - 1
            : points.Length;

        for (int i = 0; i < segmentPointCount - 1; i++)
        {
            yield return new LineSegment2D(points[i], points[i + 1]);
        }

        if (IsClosed && segmentPointCount > 2)
        {
            yield return new LineSegment2D(points[segmentPointCount - 1], points[0]);
        }
    }

    private static bool AreAdjacentSegments(int left, int right, int segmentCount)
    {
        return Math.Abs(left - right) == 1 || (left == 0 && right == segmentCount - 1);
    }

    private static bool AreSamePoint(Point2D left, Point2D right)
    {
        return left.DistanceTo(right) <= GeometryTolerance.Default;
    }
}
