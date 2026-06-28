using LaserCad.Core.Parameters;

namespace LaserCad.Core.Expressions;

/// <summary>
/// Wyrazenie odwolujace sie do parametru przez ParameterId.
/// Podczas ewaluacji evaluator pobiera wartosc z ParameterSet.
/// </summary>
public sealed class ParameterReferenceExpression : Expression
{
    /// <summary>
    /// Tworzy referencje do parametru.
    /// Uzywaj identyfikatora stabilnego, takiego samego jak w ParameterSet.
    /// </summary>
    public ParameterReferenceExpression(ParameterId parameterId)
    {
        ParameterId = parameterId;
    }

    /// <summary>
    /// Identyfikator parametru, ktorego wartosc ma zostac uzyta.
    /// </summary>
    public ParameterId ParameterId { get; }
}
