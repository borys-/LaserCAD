using LaserCad.Core.Documents;
using LaserCad.Core.Parameters;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.Dimensions;

/// <summary>
/// Wymiar szkicu opisujacy intencje rozmiaru wybranej encji.
/// </summary>
public sealed class Dimension
{
    /// <summary>
    /// Tworzy wymiar szkicu dla wskazanej encji.
    /// </summary>
    public Dimension(
        Guid entityId,
        DimensionKind kind,
        Length value,
        Guid? id = null,
        string name = "Dimension",
        ParameterId? parameterId = null)
    {
        if (entityId == Guid.Empty)
        {
            throw new ArgumentException("Dimension entity id cannot be empty.", nameof(entityId));
        }

        if (id == Guid.Empty)
        {
            throw new ArgumentException("Dimension id cannot be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Dimension name cannot be empty.", nameof(name));
        }

        if (value.Millimeters <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Dimension value must be positive.");
        }

        Id = id ?? Guid.NewGuid();
        EntityId = entityId;
        Kind = kind;
        Value = value;
        Name = name;
        ParameterId = parameterId;
    }

    /// <summary>
    /// Stabilny identyfikator wymiaru.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identyfikator encji, ktorej dotyczy wymiar.
    /// </summary>
    public Guid EntityId { get; }

    /// <summary>
    /// Rodzaj wymiaru.
    /// </summary>
    public DimensionKind Kind { get; }

    /// <summary>
    /// Nazwa wymiaru wyswietlana w UI lub diagnostyce.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Wartosc wymiaru w milimetrach domenowych.
    /// </summary>
    public Length Value { get; }

    /// <summary>
    /// Opcjonalny parametr dokumentu sterujacy wymiarem.
    /// </summary>
    public ParameterId? ParameterId { get; }

    /// <summary>
    /// Zwraca wymiar powiazany z parametrem dokumentu.
    /// </summary>
    public Dimension BindToParameter(ParameterId parameterId)
    {
        return new Dimension(EntityId, Kind, Value, Id, Name, parameterId);
    }
}
