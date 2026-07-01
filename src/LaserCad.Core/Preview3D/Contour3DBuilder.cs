using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Preview3D;

/// <summary>
/// Buduje model czesci 3D z prostych konturow 2D.
/// </summary>
public sealed class Contour3DBuilder
{
    /// <summary>
    /// Tworzy czesc z prostokata przez ekstrudowanie o zadana grubosc.
    /// </summary>
    public Part3D FromRectangle(RectangleEntity rectangle, double thicknessMillimeters, string? name = null)
    {
        if (rectangle is null)
        {
            throw new ArgumentNullException(nameof(rectangle));
        }

        return FromPolygon(rectangle.Corners, thicknessMillimeters, name ?? "Rectangle part");
    }

    /// <summary>
    /// Tworzy czesc z zamknietej polilinii przez ekstrudowanie o zadana grubosc.
    /// </summary>
    public Part3D FromPolyline(PolylineEntity polyline, double thicknessMillimeters, string? name = null)
    {
        if (polyline is null)
        {
            throw new ArgumentNullException(nameof(polyline));
        }

        if (!polyline.Polyline.IsClosed)
        {
            throw new ArgumentException("Polyline must be closed to create a 3D part.", nameof(polyline));
        }

        return FromPolygon(polyline.Polyline.Points, thicknessMillimeters, name ?? "Polyline part");
    }

    /// <summary>
    /// Tworzy czesc z polygonu 2D przez ekstrudowanie o zadana grubosc.
    /// </summary>
    public Part3D FromPolygon(IEnumerable<Point2D> points, double thicknessMillimeters, string name)
    {
        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        if (thicknessMillimeters <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(thicknessMillimeters), "Thickness must be positive.");
        }

        var polygonPoints = RemoveClosingPoint(points.ToArray());
        if (polygonPoints.Length < 3)
        {
            throw new ArgumentException("At least three unique points are required.", nameof(points));
        }

        _ = new Polygon2D(polygonPoints);

        var vertices = new List<Point3D>(polygonPoints.Length * 2);
        foreach (var point in polygonPoints)
        {
            vertices.Add(new Point3D(point.X, point.Y, 0.0));
        }

        foreach (var point in polygonPoints)
        {
            vertices.Add(new Point3D(point.X, point.Y, thicknessMillimeters));
        }

        var triangles = BuildTriangleIndices(polygonPoints.Length);
        return new Part3D(name, new Mesh3D(vertices, triangles), thicknessMillimeters);
    }

    private static int[] BuildTriangleIndices(int pointCount)
    {
        var triangles = new List<int>();

        for (var index = 1; index < pointCount - 1; index++)
        {
            triangles.Add(0);
            triangles.Add(index);
            triangles.Add(index + 1);

            triangles.Add(pointCount);
            triangles.Add(pointCount + index + 1);
            triangles.Add(pointCount + index);
        }

        for (var index = 0; index < pointCount; index++)
        {
            var next = (index + 1) % pointCount;
            var bottomA = index;
            var bottomB = next;
            var topA = pointCount + index;
            var topB = pointCount + next;

            triangles.Add(bottomA);
            triangles.Add(bottomB);
            triangles.Add(topB);

            triangles.Add(bottomA);
            triangles.Add(topB);
            triangles.Add(topA);
        }

        return triangles.ToArray();
    }

    private static Point2D[] RemoveClosingPoint(Point2D[] points)
    {
        if (points.Length > 1 && points[0].DistanceTo(points[points.Length - 1]) <= GeometryTolerance.Default)
        {
            return points.Take(points.Length - 1).ToArray();
        }

        return points;
    }
}
