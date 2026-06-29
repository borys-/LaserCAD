using LaserCad.Core.Documents;

namespace LaserCad.Core.Constraints;

/// <summary>
/// Bazowy kontrakt constraintu geometrycznego szkicu.
/// Constraint opisuje intencje geometryczna i potrafi zwrocic nowy szkic po zastosowaniu.
/// </summary>
public interface ISketchConstraint
{
    /// <summary>
    /// Stabilny identyfikator constraintu.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Rodzaj constraintu.
    /// </summary>
    SketchConstraintKind Kind { get; }

    /// <summary>
    /// Zwraca nowy szkic po zastosowaniu constraintu.
    /// </summary>
    Sketch Apply(Sketch sketch);
}
