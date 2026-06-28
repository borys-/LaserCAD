namespace LaserCad.Geometry;

/// <summary>
/// Punkt w przestrzeni 2D zapisany w milimetrach domenowych.
/// Uzywaj go do przechowywania pozycji geometrycznych w szkicach i algorytmach.
/// </summary>
public readonly record struct Point2D(double X, double Y)
{
    /// <summary>
    /// Oblicza odleglosc euklidesowa do innego punktu.
    /// Uzywaj do pomiarow dlugosci odcinkow i dystansow w geometrii 2D.
    /// </summary>
    public double DistanceTo(Point2D other)
    {
        return (this - other).Length;
    }

    /// <summary>
    /// Przesuwa punkt o wektor.
    /// Uzywaj do translacji pojedynczego punktu bez tworzenia macierzy.
    /// </summary>
    public static Point2D operator +(Point2D point, Vector2D vector)
    {
        return new Point2D(point.X + vector.X, point.Y + vector.Y);
    }

    /// <summary>
    /// Przesuwa punkt przeciwnie do kierunku wektora.
    /// </summary>
    public static Point2D operator -(Point2D point, Vector2D vector)
    {
        return new Point2D(point.X - vector.X, point.Y - vector.Y);
    }

    /// <summary>
    /// Zwraca wektor od prawego punktu do lewego punktu.
    /// Uzywaj do wyznaczania kierunku albo roznicy pozycji.
    /// </summary>
    public static Vector2D operator -(Point2D left, Point2D right)
    {
        return new Vector2D(left.X - right.X, left.Y - right.Y);
    }
}
