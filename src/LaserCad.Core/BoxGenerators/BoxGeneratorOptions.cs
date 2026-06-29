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
    public BoxGeneratorOptions(Length? width = null)
    {
        Width = width ?? Length.FromMillimeters(100.0);
    }

    /// <summary>
    /// Szerokosc pudelka mierzona w milimetrach.
    /// </summary>
    public Length Width { get; }
}
