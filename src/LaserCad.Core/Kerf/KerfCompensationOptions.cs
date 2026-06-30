using LaserCad.Geometry.Units;

namespace LaserCad.Core.Kerf;

/// <summary>
/// Opcje kompensacji kerfu dla prostych konturow MVP.
/// </summary>
public sealed class KerfCompensationOptions
{
    /// <summary>
    /// Tworzy opcje kompensacji kerfu.
    /// </summary>
    public KerfCompensationOptions(Length kerf, KerfCompensationMode mode = KerfCompensationMode.Positive)
    {
        if (kerf.Millimeters < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(kerf), "Kerf must be non-negative.");
        }

        Kerf = kerf;
        Mode = mode;
    }

    /// <summary>
    /// Szerokosc szczeliny ciecia.
    /// </summary>
    public Length Kerf { get; }

    /// <summary>
    /// Tryb kierunku kompensacji.
    /// </summary>
    public KerfCompensationMode Mode { get; }

    /// <summary>
    /// Odleglosc offsetu od linii nominalnej.
    /// </summary>
    public double OffsetDistanceMillimeters => Kerf.Millimeters / 2.0;
}
