namespace LaserCad.Geometry;

/// <summary>
/// Wektor 2D opisujacy kierunek i przesuniecie w geometrii.
/// Uzywaj go do operacji na kierunkach, przesunieciach oraz obliczen pomocniczych.
/// </summary>
public readonly record struct Vector2D(double X, double Y)
{
    /// <summary>
    /// Dlugosc euklidesowa wektora.
    /// </summary>
    public double Length => Math.Sqrt((X * X) + (Y * Y));

    /// <summary>
    /// Zwraca wektor jednostkowy o tym samym kierunku.
    /// Nie uzywaj dla wektora zerowego, bo metoda zglosi blad.
    /// </summary>
    public Vector2D Normalize()
    {
        double length = Length;

        if (length <= GeometryTolerance.Default)
        {
            throw new InvalidOperationException("Zero length vector cannot be normalized.");
        }

        return new Vector2D(X / length, Y / length);
    }

    /// <summary>
    /// Oblicza iloczyn skalarny z innym wektorem.
    /// Uzywaj do rzutowania, porownywania kierunkow i obliczania katow.
    /// </summary>
    public double Dot(Vector2D other)
    {
        return (X * other.X) + (Y * other.Y);
    }

    /// <summary>
    /// Oblicza iloczyn wektorowy 2D jako wartosc skalarna.
    /// Uzywaj do okreslania orientacji i skretu miedzy wektorami.
    /// </summary>
    public double Cross(Vector2D other)
    {
        return (X * other.Y) - (Y * other.X);
    }

    /// <summary>
    /// Zwraca kat w radianach miedzy tym wektorem i innym wektorem.
    /// Oba wektory musza miec niezerowa dlugosc.
    /// </summary>
    public double AngleTo(Vector2D other)
    {
        Vector2D left = Normalize();
        Vector2D right = other.Normalize();
        double dot = Math.Clamp(left.Dot(right), -1.0, 1.0);

        return Math.Acos(dot);
    }
}
