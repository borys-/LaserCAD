namespace LaserCad.Geometry;

/// <summary>
/// Operacje offsetu dla prostych polygonow.
/// MVP obsluguje wylacznie proste polygony wypukle z krawedziami liniowymi.
/// </summary>
public static class PolygonOffset2D
{
    /// <summary>
    /// Zwraca offset zewnetrzny polygonu wypuklego o podana odleglosc w milimetrach.
    /// </summary>
    public static Polygon2D OffsetOuter(Polygon2D polygon, double distanceMillimeters)
    {
        return Offset(polygon, distanceMillimeters, outward: true);
    }

    /// <summary>
    /// Zwraca offset wewnetrzny polygonu wypuklego o podana odleglosc w milimetrach.
    /// </summary>
    public static Polygon2D OffsetInner(Polygon2D polygon, double distanceMillimeters)
    {
        return Offset(polygon, distanceMillimeters, outward: false);
    }

    private static Polygon2D Offset(Polygon2D polygon, double distanceMillimeters, bool outward)
    {
        if (polygon is null)
        {
            throw new ArgumentNullException(nameof(polygon));
        }

        if (distanceMillimeters < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(distanceMillimeters), "Offset distance must be non-negative.");
        }

        if (distanceMillimeters <= GeometryTolerance.Default)
        {
            return new Polygon2D(polygon.Vertices);
        }

        Contour2D contour = Contour2D.FromPolygon(polygon);

        if (contour.HasSelfIntersections())
        {
            throw new ArgumentException("Only simple polygons can be offset.", nameof(polygon));
        }

        if (!IsConvex(polygon))
        {
            throw new ArgumentException("Only convex polygons can be offset in MVP.", nameof(polygon));
        }

        IReadOnlyList<Point2D> vertices = polygon.Vertices;
        Line2D[] offsetLines = new Line2D[vertices.Count];

        for (int i = 0; i < vertices.Count; i++)
        {
            Point2D start = vertices[i];
            Point2D end = vertices[(i + 1) % vertices.Count];
            Vector2D direction = (end - start).Normalize();
            Vector2D normal = GetOffsetNormal(direction, polygon.Orientation, outward);
            Point2D shiftedStart = start + Scale(normal, distanceMillimeters);

            offsetLines[i] = new Line2D(shiftedStart, direction);
        }

        Point2D[] offsetVertices = new Point2D[vertices.Count];

        for (int i = 0; i < offsetLines.Length; i++)
        {
            Line2D previous = offsetLines[(i - 1 + offsetLines.Length) % offsetLines.Length];
            Line2D current = offsetLines[i];
            IntersectionResult intersection = Intersections2D.Intersect(previous, current);

            if (!intersection.IsPoint || intersection.Point is null)
            {
                throw new InvalidOperationException("Offset edges did not produce a valid polygon vertex.");
            }

            offsetVertices[i] = intersection.Point.Value;
        }

        return new Polygon2D(offsetVertices);
    }

    private static Vector2D GetOffsetNormal(Vector2D direction, PolygonOrientation orientation, bool outward)
    {
        Vector2D left = new Vector2D(-direction.Y, direction.X);
        Vector2D right = new Vector2D(direction.Y, -direction.X);
        bool interiorIsLeft = orientation == PolygonOrientation.Counterclockwise;
        Vector2D outwardNormal = interiorIsLeft ? right : left;
        Vector2D inwardNormal = interiorIsLeft ? left : right;

        return outward ? outwardNormal : inwardNormal;
    }

    private static bool IsConvex(Polygon2D polygon)
    {
        IReadOnlyList<Point2D> vertices = polygon.Vertices;
        double expectedSign = polygon.Orientation == PolygonOrientation.Counterclockwise ? 1.0 : -1.0;

        for (int i = 0; i < vertices.Count; i++)
        {
            Vector2D previous = vertices[i] - vertices[(i - 1 + vertices.Count) % vertices.Count];
            Vector2D next = vertices[(i + 1) % vertices.Count] - vertices[i];
            double cross = previous.Cross(next);

            if (Math.Abs(cross) <= GeometryTolerance.Default)
            {
                throw new ArgumentException("Polygon must not contain collinear adjacent edges.", nameof(polygon));
            }

            if (Math.Sign(cross) != Math.Sign(expectedSign))
            {
                return false;
            }
        }

        return true;
    }

    private static Vector2D Scale(Vector2D vector, double scale)
    {
        return new Vector2D(vector.X * scale, vector.Y * scale);
    }
}
