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
        bool startWithFinger = true)
    {
        FingerWidth = fingerWidth ?? Length.FromMillimeters(0.0);
        MinimumFingerWidth = minimumFingerWidth ?? Length.FromMillimeters(0.0);
        MaximumFingerWidth = maximumFingerWidth ?? Length.FromMillimeters(0.0);
        StartWithFinger = startWithFinger;
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
}
