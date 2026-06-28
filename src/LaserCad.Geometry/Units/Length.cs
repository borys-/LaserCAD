using System.Globalization;
using LaserCad.Geometry;

namespace LaserCad.Geometry.Units;

/// <summary>
/// Silny typ reprezentujacy dlugosc przechowywana wewnetrznie w milimetrach.
/// Uzywaj go w publicznym API zawsze wtedy, gdy wartosc oznacza fizyczny wymiar.
/// </summary>
public readonly record struct Length : IComparable<Length>, IFormattable
{
    private const double MillimetersPerCentimeter = 10.0;
    private const double MillimetersPerInch = 25.4;

    private readonly double _millimeters;

    /// <summary>
    /// Tworzy dlugosc z wartosci juz przeliczonej na milimetry.
    /// Uzywaj publicznych fabryk, aby jawnie wskazac jednostke wartosci wejsciowej.
    /// </summary>
    private Length(double millimeters)
    {
        _millimeters = millimeters;
    }

    /// <summary>
    /// Wartosc dlugosci w milimetrach.
    /// Uzywaj do obliczen i eksportu, gdzie wymagana jest jawna jednostka bazowa.
    /// </summary>
    public double Millimeters => _millimeters;

    /// <summary>
    /// Tworzy dlugosc z wartosci podanej w milimetrach.
    /// </summary>
    public static Length FromMillimeters(double millimeters)
    {
        return new Length(millimeters);
    }

    /// <summary>
    /// Tworzy dlugosc z wartosci podanej w centymetrach.
    /// Wartosc zostanie przeliczona na milimetry.
    /// </summary>
    public static Length FromCentimeters(double centimeters)
    {
        return new Length(centimeters * MillimetersPerCentimeter);
    }

    /// <summary>
    /// Tworzy dlugosc z wartosci podanej w calach.
    /// Wartosc zostanie przeliczona na milimetry.
    /// </summary>
    public static Length FromInches(double inches)
    {
        return new Length(inches * MillimetersPerInch);
    }

    /// <summary>
    /// Sprawdza przyblizona rownosc z inna dlugoscia przy podanej tolerancji.
    /// Uzywaj zamiast bezposredniego porownania double.
    /// </summary>
    public bool IsApproximatelyEqualTo(Length other, double tolerance = GeometryTolerance.Default)
    {
        if (tolerance < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(tolerance), "Tolerance cannot be negative.");
        }

        return Math.Abs(_millimeters - other._millimeters) <= tolerance;
    }

    /// <summary>
    /// Porownuje dwie dlugosci z uwzglednieniem domyslnej tolerancji.
    /// Zwraca 0 dla wartosci uznanych za rowne.
    /// </summary>
    public int CompareTo(Length other)
    {
        if (IsApproximatelyEqualTo(other))
        {
            return 0;
        }

        return _millimeters < other._millimeters ? -1 : 1;
    }

    /// <summary>
    /// Formatuje dlugosc jako tekst w milimetrach z domyslnym formatem liczby.
    /// </summary>
    public override string ToString()
    {
        return ToString("0.###", CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Formatuje dlugosc jako tekst w milimetrach z podanym formatem i kultura.
    /// Uzywaj, gdy UI albo eksport wymaga konkretnej liczby miejsc po przecinku.
    /// </summary>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        var value = _millimeters.ToString(format ?? "0.###", formatProvider ?? CultureInfo.InvariantCulture);

        return $"{value} mm";
    }

    /// <summary>
    /// Dodaje dwie dlugosci.
    /// </summary>
    public static Length operator +(Length left, Length right)
    {
        return new Length(left._millimeters + right._millimeters);
    }

    /// <summary>
    /// Odejmuje prawa dlugosc od lewej.
    /// </summary>
    public static Length operator -(Length left, Length right)
    {
        return new Length(left._millimeters - right._millimeters);
    }

    /// <summary>
    /// Mnozy dlugosc przez liczbe.
    /// </summary>
    public static Length operator *(Length length, double multiplier)
    {
        return new Length(length._millimeters * multiplier);
    }

    /// <summary>
    /// Mnozy liczbe przez dlugosc.
    /// </summary>
    public static Length operator *(double multiplier, Length length)
    {
        return length * multiplier;
    }

    /// <summary>
    /// Dzieli dlugosc przez liczbe.
    /// Nie przekazuj zera jako dzielnika.
    /// </summary>
    public static Length operator /(Length length, double divisor)
    {
        if (divisor == 0.0)
        {
            throw new DivideByZeroException("Length cannot be divided by zero.");
        }

        return new Length(length._millimeters / divisor);
    }

    /// <summary>
    /// Sprawdza, czy lewa dlugosc jest mniejsza od prawej z uwzglednieniem tolerancji.
    /// </summary>
    public static bool operator <(Length left, Length right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Sprawdza, czy lewa dlugosc jest mniejsza albo rowna prawej z uwzglednieniem tolerancji.
    /// </summary>
    public static bool operator <=(Length left, Length right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Sprawdza, czy lewa dlugosc jest wieksza od prawej z uwzglednieniem tolerancji.
    /// </summary>
    public static bool operator >(Length left, Length right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Sprawdza, czy lewa dlugosc jest wieksza albo rowna prawej z uwzglednieniem tolerancji.
    /// </summary>
    public static bool operator >=(Length left, Length right)
    {
        return left.CompareTo(right) >= 0;
    }
}
