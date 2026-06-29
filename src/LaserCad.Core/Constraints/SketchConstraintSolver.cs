using LaserCad.Core.Documents;

namespace LaserCad.Core.Constraints;

/// <summary>
/// Prosty solver constraintow szkicu dla zakresu MVP.
/// Stosuje constrainty sekwencyjnie i zwraca przebudowany szkic.
/// </summary>
public sealed class SketchConstraintSolver
{
    /// <summary>
    /// Zwraca nowy szkic po zastosowaniu wszystkich constraintow w przekazanej kolejnosci.
    /// </summary>
    public Sketch Solve(Sketch sketch, IEnumerable<ISketchConstraint> constraints)
    {
        if (sketch is null)
        {
            throw new ArgumentNullException(nameof(sketch));
        }

        if (constraints is null)
        {
            throw new ArgumentNullException(nameof(constraints));
        }

        var solved = sketch;

        foreach (var constraint in constraints)
        {
            if (constraint is null)
            {
                throw new ArgumentException("Constraints cannot contain null values.", nameof(constraints));
            }

            solved = constraint.Apply(solved);
        }

        return solved;
    }
}
