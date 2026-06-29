namespace LaserCad.Core.Constraints;

/// <summary>
/// Rodzaj constraintu geometrycznego w szkicu.
/// </summary>
public enum SketchConstraintKind
{
    /// <summary>
    /// Ograniczenie wymuszajace poziomy odcinek.
    /// </summary>
    Horizontal,

    /// <summary>
    /// Ograniczenie wymuszajace pionowy odcinek.
    /// </summary>
    Vertical,

    /// <summary>
    /// Ograniczenie wymuszajace rownoleglosc dwoch odcinkow.
    /// </summary>
    Parallel,

    /// <summary>
    /// Ograniczenie wymuszajace prostopadlosc dwoch odcinkow.
    /// </summary>
    Perpendicular,

    /// <summary>
    /// Ograniczenie laczace dwa punkty geometryczne.
    /// </summary>
    Coincident,

    /// <summary>
    /// Ograniczenie wymuszajace rownosc wymiaru dwoch encji.
    /// </summary>
    Equal,
}
