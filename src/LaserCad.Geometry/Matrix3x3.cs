namespace LaserCad.Geometry;

/// <summary>
/// Macierz 3x3 do transformacji afinicznych 2D we wspolrzednych jednorodnych.
/// Uzywaj jej do laczenia przesuniecia, obrotu, skalowania i odbicia.
/// </summary>
public readonly record struct Matrix3x3(
    double M11,
    double M12,
    double M13,
    double M21,
    double M22,
    double M23,
    double M31,
    double M32,
    double M33)
{
    /// <summary>
    /// Macierz jednostkowa, ktora nie zmienia punktow ani wektorow.
    /// </summary>
    public static Matrix3x3 Identity { get; } = new Matrix3x3(
        1.0, 0.0, 0.0,
        0.0, 1.0, 0.0,
        0.0, 0.0, 1.0);

    /// <summary>
    /// Tworzy macierz przesuniecia o podane wartosci X i Y.
    /// Uzywaj do przesuwania punktow w plaszczyznie.
    /// </summary>
    public static Matrix3x3 CreateTranslation(double offsetX, double offsetY)
    {
        return new Matrix3x3(
            1.0, 0.0, offsetX,
            0.0, 1.0, offsetY,
            0.0, 0.0, 1.0);
    }

    /// <summary>
    /// Tworzy macierz obrotu o kat w radianach wokol poczatku ukladu.
    /// </summary>
    public static Matrix3x3 CreateRotation(double angleRadians)
    {
        double cos = Math.Cos(angleRadians);
        double sin = Math.Sin(angleRadians);

        return new Matrix3x3(
            cos, -sin, 0.0,
            sin, cos, 0.0,
            0.0, 0.0, 1.0);
    }

    /// <summary>
    /// Tworzy macierz skalowania niezaleznie w osi X i Y.
    /// </summary>
    public static Matrix3x3 CreateScaling(double scaleX, double scaleY)
    {
        return new Matrix3x3(
            scaleX, 0.0, 0.0,
            0.0, scaleY, 0.0,
            0.0, 0.0, 1.0);
    }

    /// <summary>
    /// Tworzy macierz odbicia wzgledem osi X.
    /// </summary>
    public static Matrix3x3 CreateReflectionX()
    {
        return CreateScaling(1.0, -1.0);
    }

    /// <summary>
    /// Tworzy macierz odbicia wzgledem osi Y.
    /// </summary>
    public static Matrix3x3 CreateReflectionY()
    {
        return CreateScaling(-1.0, 1.0);
    }

    /// <summary>
    /// Transformuje punkt, uwzgledniajac translacje i wspolrzedna jednorodna.
    /// Uzywaj dla pozycji geometrycznych.
    /// </summary>
    public Point2D Transform(Point2D point)
    {
        double x = (M11 * point.X) + (M12 * point.Y) + M13;
        double y = (M21 * point.X) + (M22 * point.Y) + M23;
        double w = (M31 * point.X) + (M32 * point.Y) + M33;

        if (Math.Abs(w) <= GeometryTolerance.Default)
        {
            throw new InvalidOperationException("Point transform produced an invalid homogeneous coordinate.");
        }

        return new Point2D(x / w, y / w);
    }

    /// <summary>
    /// Transformuje wektor bez translacji.
    /// Uzywaj dla kierunkow i przesuniec, ktore nie maja pozycji absolutnej.
    /// </summary>
    public Vector2D Transform(Vector2D vector)
    {
        double x = (M11 * vector.X) + (M12 * vector.Y);
        double y = (M21 * vector.X) + (M22 * vector.Y);

        return new Vector2D(x, y);
    }

    /// <summary>
    /// Mnozy dwie macierze, skladajac ich transformacje.
    /// Wynik stosuj tak, jak zlozona transformacje lewego i prawego argumentu.
    /// </summary>
    public static Matrix3x3 operator *(Matrix3x3 left, Matrix3x3 right)
    {
        return new Matrix3x3(
            (left.M11 * right.M11) + (left.M12 * right.M21) + (left.M13 * right.M31),
            (left.M11 * right.M12) + (left.M12 * right.M22) + (left.M13 * right.M32),
            (left.M11 * right.M13) + (left.M12 * right.M23) + (left.M13 * right.M33),
            (left.M21 * right.M11) + (left.M22 * right.M21) + (left.M23 * right.M31),
            (left.M21 * right.M12) + (left.M22 * right.M22) + (left.M23 * right.M32),
            (left.M21 * right.M13) + (left.M22 * right.M23) + (left.M23 * right.M33),
            (left.M31 * right.M11) + (left.M32 * right.M21) + (left.M33 * right.M31),
            (left.M31 * right.M12) + (left.M32 * right.M22) + (left.M33 * right.M32),
            (left.M31 * right.M13) + (left.M32 * right.M23) + (left.M33 * right.M33));
    }
}
