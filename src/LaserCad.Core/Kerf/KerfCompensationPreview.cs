using LaserCad.Core.Documents;

namespace LaserCad.Core.Kerf;

/// <summary>
/// Wynik podgladu kompensacji kerfu: geometria nominalna i skompensowana.
/// </summary>
public sealed class KerfCompensationPreview
{
    /// <summary>
    /// Tworzy wynik podgladu kompensacji.
    /// </summary>
    public KerfCompensationPreview(Sketch beforeCompensation, Sketch afterCompensation)
    {
        BeforeCompensation = beforeCompensation ?? throw new ArgumentNullException(nameof(beforeCompensation));
        AfterCompensation = afterCompensation ?? throw new ArgumentNullException(nameof(afterCompensation));
    }

    /// <summary>
    /// Szkic przed kompensacja.
    /// </summary>
    public Sketch BeforeCompensation { get; }

    /// <summary>
    /// Szkic po kompensacji.
    /// </summary>
    public Sketch AfterCompensation { get; }
}
