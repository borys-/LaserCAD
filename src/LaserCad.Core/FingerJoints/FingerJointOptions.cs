using LaserCad.Geometry.Units;

namespace LaserCad.Core.FingerJoints;

/// <summary>
/// Opcje opisujace parametry polaczenia palcowego dla generatorow produkcyjnych.
/// </summary>
public sealed class FingerJointOptions
{
    /// <summary>
    /// Tworzy opcje polaczenia palcowego z wartosciami domyslnymi bez kompensacji.
    /// </summary>
    public FingerJointOptions(
        Length? fingerWidth = null,
        Length? minimumFingerWidth = null,
        Length? maximumFingerWidth = null,
        bool startWithFinger = true,
        bool endWithFinger = true,
        FingerJointFitMode fitMode = FingerJointFitMode.Neutral,
        Length? kerf = null,
        Length? clearance = null)
    {
        FingerWidth = EnsurePositive(fingerWidth ?? Length.FromMillimeters(1.0), nameof(fingerWidth), "Finger width must be greater than zero.");
        MinimumFingerWidth = EnsurePositive(minimumFingerWidth ?? FingerWidth, nameof(minimumFingerWidth), "Minimum finger width must be greater than zero.");
        MaximumFingerWidth = EnsurePositive(maximumFingerWidth ?? FingerWidth, nameof(maximumFingerWidth), "Maximum finger width must be greater than zero.");
        StartWithFinger = startWithFinger;
        EndWithFinger = endWithFinger;
        FitMode = fitMode;
        Kerf = EnsureNonNegative(kerf ?? Length.FromMillimeters(0.0), nameof(kerf), "Kerf cannot be negative.");
        Clearance = EnsureNonNegative(clearance ?? Length.FromMillimeters(0.0), nameof(clearance), "Clearance cannot be negative.");

        if (MinimumFingerWidth > MaximumFingerWidth)
        {
            throw new ArgumentException("Minimum finger width cannot be greater than maximum finger width.", nameof(minimumFingerWidth));
        }

        if (FingerWidth < MinimumFingerWidth || FingerWidth > MaximumFingerWidth)
        {
            throw new ArgumentOutOfRangeException(nameof(fingerWidth), "Finger width must be within minimum and maximum finger width.");
        }
    }

    /// <summary>
    /// Docelowa szerokosc pojedynczego palca.
    /// </summary>
    public Length FingerWidth { get; }

    /// <summary>
    /// Minimalna dopuszczalna szerokosc palca przy automatycznym doborze podzialu.
    /// </summary>
    public Length MinimumFingerWidth { get; }

    /// <summary>
    /// Maksymalna dopuszczalna szerokosc palca przy automatycznym doborze podzialu.
    /// </summary>
    public Length MaximumFingerWidth { get; }

    /// <summary>
    /// Okresla, czy profil krawedzi ma zaczynac sie od wystajacego palca.
    /// </summary>
    public bool StartWithFinger { get; }

    /// <summary>
    /// Okresla, czy profil krawedzi ma konczyc sie wystajacym palcem.
    /// </summary>
    public bool EndWithFinger { get; }

    /// <summary>
    /// Tryb dopasowania polaczenia palcowego.
    /// </summary>
    public FingerJointFitMode FitMode { get; }

    /// <summary>
    /// Szerokosc szczeliny ciecia lasera uwzgledniana przez generator.
    /// </summary>
    public Length Kerf { get; }

    /// <summary>
    /// Dodatkowy luz montazowy dla polaczenia.
    /// </summary>
    public Length Clearance { get; }

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
}
