using LaserCad.Core.Parameters;

namespace LaserCad.Core.Documents;

/// <summary>
/// Powiazanie wymiaru encji szkicu z parametrem dokumentu.
/// </summary>
public sealed class EntityDimensionBinding
{
    /// <summary>
    /// Tworzy powiazanie wymiaru encji z parametrem.
    /// </summary>
    public EntityDimensionBinding(EntityDimensionKind dimension, ParameterId parameterId)
    {
        Dimension = dimension;
        ParameterId = parameterId;
    }

    /// <summary>
    /// Wymiar encji sterowany parametrem.
    /// </summary>
    public EntityDimensionKind Dimension { get; }

    /// <summary>
    /// Identyfikator parametru, ktorego wartosc steruje wymiarem.
    /// </summary>
    public ParameterId ParameterId { get; }
}
