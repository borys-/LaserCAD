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

    /// <summary>
    /// Oblicza przeciecie dwoch odcinkow.
    /// </summary>
    public static IntersectionResult Intersect(LineSegment2D left, LineSegment2D right)
    {
        Vector2D p = left.Start - new Point2D(0.0, 0.0);
        Vector2D r = left.End - left.Start;
        Vector2D q = right.Start - new Point2D(0.0, 0.0);
        Vector2D s = right.End - right.Start;
        double denominator = r.Cross(s);
        Vector2D difference = new Vector2D(q.X - p.X, q.Y - p.Y);

        if (IsZero(denominator))
        {
            if (!IsZero(difference.Cross(r)))
            {
                return IntersectionResult.Parallel();
            }

            return IntersectCollinearSegments(left, right, r);
        }

        double t = difference.Cross(s) / denominator;
        double u = difference.Cross(r) / denominator;

        if (IsWithinUnitRange(t) && IsWithinUnitRange(u))
        {
            return IntersectionResult.FromPoint(left.PointAt(ClampUnit(t)));
        }

        return IntersectionResult.None();
    }

    private static bool IsZero(double value)
    {
        return Math.Abs(value) <= GeometryTolerance.Default;
    }

    private static IntersectionResult IntersectCollinearSegments(
        LineSegment2D left,
        LineSegment2D right,
        Vector2D leftDirection)
    {
        double leftLengthSquared = leftDirection.Dot(leftDirection);

        if (IsZero(leftLengthSquared))
        {
            return PointOnSegment(left.Start, right)
                ? IntersectionResult.FromPoint(left.Start)
                : IntersectionResult.None();
        }

        double start = ParameterOnLine(left.Start, right.Start, leftDirection, leftLengthSquared);
        double end = ParameterOnLine(left.Start, right.End, leftDirection, leftLengthSquared);
        double overlapStart = Math.Max(0.0, Math.Min(start, end));
        double overlapEnd = Math.Min(1.0, Math.Max(start, end));

        if (overlapStart > overlapEnd + GeometryTolerance.Default)
        {
            return IntersectionResult.None();
        }

        if (Math.Abs(overlapStart - overlapEnd) <= GeometryTolerance.Default)
        {
            return IntersectionResult.FromPoint(left.PointAt(ClampUnit(overlapStart)));
        }

        return IntersectionResult.FromOverlap(
            new LineSegment2D(left.PointAt(ClampUnit(overlapStart)), left.PointAt(ClampUnit(overlapEnd))));
    }

    private static double ParameterOnLine(
        Point2D origin,
        Point2D point,
        Vector2D direction,
        double directionLengthSquared)
    {
        return (point - origin).Dot(direction) / directionLengthSquared;
    }

    private static bool PointOnSegment(Point2D point, LineSegment2D segment)
    {
        Vector2D segmentDirection = segment.End - segment.Start;
        Vector2D fromStart = point - segment.Start;

        if (!IsZero(fromStart.Cross(segmentDirection)))
        {
            return false;
        }

        double lengthSquared = segmentDirection.Dot(segmentDirection);

        if (IsZero(lengthSquared))
        {
            return point.DistanceTo(segment.Start) <= GeometryTolerance.Default;
        }

        double t = fromStart.Dot(segmentDirection) / lengthSquared;

        return IsWithinUnitRange(t);
    }

    private static bool IsWithinUnitRange(double value)
    {
        return value >= -GeometryTolerance.Default && value <= 1.0 + GeometryTolerance.Default;
    }

    private static double ClampUnit(double value)
    {
        return Math.Clamp(value, 0.0, 1.0);
    }
}
