namespace LaserCad.Geometry;

/// <summary>
/// Polilinia 2D zlozona z kolejnych punktow.
/// Uzywaj jej dla otwartych i zamknietych lamanych w szkicach oraz algorytmach geometrii.
/// </summary>
public sealed class Polyline2D
{
    private readonly Point2D[] points;
    private readonly IReadOnlyList<Point2D> readOnlyPoints;

    /// <summary>
    /// Tworzy polilinie z podanej listy punktow.
    /// </summary>
    public Polyline2D(IEnumerable<Point2D> points, bool isClosed = false)
    {
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        this.points = points.ToArray();

        if (this.points.Length < 2)
        {
            throw new ArgumentException("Polyline must contain at least two points.", nameof(points));
        }

        readOnlyPoints = Array.AsReadOnly(this.points);
        IsClosed = isClosed;
    }

    /// <summary>
    /// Punkty polilinii w kolejnosci przebiegu.
    /// </summary>
    public IReadOnlyList<Point2D> Points => readOnlyPoints;

    /// <summary>
    /// Informuje, czy ostatni punkt jest laczony odcinkiem z pierwszym.
    /// </summary>
    public bool IsClosed { get; }

    /// <summary>
    /// Dlugosc polilinii w milimetrach domenowych.
    /// Dla polilinii zamknietej obejmuje takze odcinek od ostatniego punktu do pierwszego.
    /// </summary>
    public double Length
    {
        get
        {
            double length = 0.0;

            for (int i = 0; i < points.Length - 1; i++)
            {
                length += points[i].DistanceTo(points[i + 1]);
            }

            if (IsClosed)
            {
                length += points[^1].DistanceTo(points[0]);
            }

            return length;
        }
    }

    /// <summary>
    /// Bounding box obejmujacy wszystkie punkty polilinii.
    /// </summary>
    public BoundingBox Bounds => BoundingBox.FromPoints(points);

    /// <summary>
    /// Zwraca polilinie po zastosowaniu transformacji afinicznej do wszystkich punktow.
    /// </summary>
    public Polyline2D Transform(Matrix3x3 transform)
    {
        return new Polyline2D(points.Select(transform.Transform), IsClosed);
    }
}
