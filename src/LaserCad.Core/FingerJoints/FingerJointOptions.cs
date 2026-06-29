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
        FingerWidth = fingerWidth ?? Length.FromMillimeters(0.0);
        MinimumFingerWidth = minimumFingerWidth ?? Length.FromMillimeters(0.0);
        MaximumFingerWidth = maximumFingerWidth ?? Length.FromMillimeters(0.0);
        StartWithFinger = startWithFinger;
        EndWithFinger = endWithFinger;
        FitMode = fitMode;
        Kerf = kerf ?? Length.FromMillimeters(0.0);
        Clearance = clearance ?? Length.FromMillimeters(0.0);
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
}
