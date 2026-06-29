using LaserCad.Geometry.Units;

namespace LaserCad.Core.BoxGenerators;

/// <summary>
/// Opcje opisujace parametry przyszlego generatora pudelka.
/// </summary>
public sealed class BoxGeneratorOptions
{
    /// <summary>
    /// Tworzy opcje generatora pudelka z domyslnymi wymiarami startowymi.
    /// </summary>
    public BoxGeneratorOptions(
        Length? width = null,
        Length? depth = null,
        Length? height = null,
        Length? materialThickness = null,
        Length? kerf = null,
        Length? fingerWidth = null,
        Length? clearance = null,
        BoxGeneratorType boxType = BoxGeneratorType.Open)
    {
        Width = EnsurePositive(width ?? Length.FromMillimeters(100.0), nameof(width), "Box width must be greater than zero.");
        Depth = EnsurePositive(depth ?? Length.FromMillimeters(80.0), nameof(depth), "Box depth must be greater than zero.");
        Height = EnsurePositive(height ?? Length.FromMillimeters(50.0), nameof(height), "Box height must be greater than zero.");
        MaterialThickness = EnsurePositive(materialThickness ?? Length.FromMillimeters(3.0), nameof(materialThickness), "Material thickness must be greater than zero.");
        Kerf = EnsureNonNegative(kerf ?? Length.FromMillimeters(0.0), nameof(kerf), "Kerf cannot be negative.");
        FingerWidth = EnsurePositive(fingerWidth ?? Length.FromMillimeters(10.0), nameof(fingerWidth), "Finger width must be greater than zero.");
        Clearance = EnsureNonNegative(clearance ?? Length.FromMillimeters(0.0), nameof(clearance), "Clearance cannot be negative.");
        BoxType = boxType;

        EnsureBoxCanContainMaterial();
    }

    /// <summary>
    /// Szerokosc pudelka mierzona w milimetrach.
    /// </summary>
    public Length Width { get; }

    /// <summary>
    /// Glebokosc pudelka mierzona w milimetrach.
    /// </summary>
    public Length Depth { get; }

    /// <summary>
    /// Wysokosc pudelka mierzona w milimetrach.
    /// </summary>
    public Length Height { get; }

    /// <summary>
    /// Grubosc materialu uzywana przez generator polaczen i scianek.
    /// </summary>
    public Length MaterialThickness { get; }

    /// <summary>
    /// Szerokosc szczeliny ciecia lasera uwzgledniana przy generowaniu pudelka.
    /// </summary>
    public Length Kerf { get; }

    /// <summary>
    /// Docelowa szerokosc palca dla polaczen palcowych scianek.
    /// </summary>
    public Length FingerWidth { get; }

    /// <summary>
    /// Dodatkowy luz montazowy dla polaczen scianek pudelka.
    /// </summary>
    public Length Clearance { get; }

    /// <summary>
    /// Wariant konstrukcyjny pudelka.
    /// </summary>
    public BoxGeneratorType BoxType { get; }

    /// <summary>
    /// Zwraca dlugosc po sprawdzeniu, ze jest dodatnia.
    /// </summary>
    private static Length EnsurePositive(Length value, string parameterName, string message)
    {
        if (value <= Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(parameterName, message);
        }

        return value;
    }

    /// <summary>
    /// Zwraca dlugosc po sprawdzeniu, ze nie jest ujemna.
    /// </summary>
    private static Length EnsureNonNegative(Length value, string parameterName, string message)
    {
        if (value < Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(parameterName, message);
        }

        return value;
    }

    /// <summary>
    /// Sprawdza minimalny rozmiar pudelka wzgledem grubosci materialu.
    /// </summary>
    private void EnsureBoxCanContainMaterial()
    {
        var minimumOuterSize = MaterialThickness * 2.0;

        if (Width <= minimumOuterSize)
        {
            throw new ArgumentOutOfRangeException(nameof(Width), "Box width must be greater than twice the material thickness.");
        }

        if (Depth <= minimumOuterSize)
        {
            throw new ArgumentOutOfRangeException(nameof(Depth), "Box depth must be greater than twice the material thickness.");
        }

        if (Height <= MaterialThickness)
        {
            throw new ArgumentOutOfRangeException(nameof(Height), "Box height must be greater than material thickness.");
        }
    }
}
