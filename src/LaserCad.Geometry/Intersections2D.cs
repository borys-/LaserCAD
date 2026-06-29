namespace LaserCad.Geometry;

/// <summary>
/// Zestaw operacji przeciec dla podstawowych obiektow 2D.
/// </summary>
public static class Intersections2D
{
    /// <summary>
    /// Oblicza przeciecie dwoch linii nieskonczonych.
    /// </summary>
    public static IntersectionResult Intersect(Line2D left, Line2D right)
    {
        double denominator = left.Direction.Cross(right.Direction);
        Vector2D difference = right.Point - left.Point;

        if (IsZero(denominator))
        {
            if (IsZero(difference.Cross(left.Direction)))
            {
                return IntersectionResult.Overlap();
            }

            return IntersectionResult.Parallel();
        }

        double t = difference.Cross(right.Direction) / denominator;
        Point2D point = left.Point + new Vector2D(left.Direction.X * t, left.Direction.Y * t);

        return IntersectionResult.FromPoint(point);
    }

    private static bool IsZero(double value)
    {
        return Math.Abs(value) <= GeometryTolerance.Default;
    }
}
