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
        Length? clearance = null)
    {
        Width = width ?? Length.FromMillimeters(100.0);
        Depth = depth ?? Length.FromMillimeters(80.0);
        Height = height ?? Length.FromMillimeters(50.0);
        MaterialThickness = materialThickness ?? Length.FromMillimeters(3.0);
        Kerf = kerf ?? Length.FromMillimeters(0.0);
        FingerWidth = fingerWidth ?? Length.FromMillimeters(10.0);
        Clearance = clearance ?? Length.FromMillimeters(0.0);
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
}
