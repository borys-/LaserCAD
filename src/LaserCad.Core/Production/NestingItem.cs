using LaserCad.Geometry.Units;

namespace LaserCad.Core.Production;

/// <summary>
/// Prostokatny element wejscowy do nestingu MVP.
/// </summary>
public sealed class NestingItem
{
    /// <summary>
    /// Tworzy element do ulozenia na arkuszu.
    /// </summary>
    public NestingItem(string name, Length width, Length height)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Nesting item name cannot be empty.", nameof(name));
        }

        Name = name;
        Width = EnsurePositive(width, nameof(width));
        Height = EnsurePositive(height, nameof(height));
    }

    /// <summary>
    /// Nazwa elementu.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Szerokosc elementu.
    /// </summary>
    public Length Width { get; }

    /// <summary>
    /// Wysokosc elementu.
    /// </summary>
    public Length Height { get; }

    private static Length EnsurePositive(Length value, string parameterName)
    {
        if (value <= Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(parameterName, "Nesting item dimension must be greater than zero.");
        }

        return value;
    }
}
