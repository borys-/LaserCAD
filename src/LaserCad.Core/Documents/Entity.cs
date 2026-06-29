using LaserCad.Geometry;

namespace LaserCad.Core.Documents;

/// <summary>
/// Bazowy kontrakt encji szkicu.
/// Dziedzicz po tym typie przy implementowaniu konkretnych encji, takich jak linia, prostokat albo okrag.
/// </summary>
public abstract class Entity : ISketchEntity
{
    private readonly EntityDimensionBinding[] dimensionBindings;

    /// <summary>
    /// Tworzy encje z opcjonalnym identyfikatorem.
    /// Konstruktor klas pochodnych powinien przekazac id przy odczycie dokumentu albo zostawic null dla nowej encji.
    /// </summary>
    protected Entity(
        Guid? id = null,
        string layerName = "Cut",
        IEnumerable<EntityDimensionBinding>? dimensionBindings = null)
    {
        if (string.IsNullOrWhiteSpace(layerName))
        {
            throw new ArgumentException("Layer name cannot be empty.", nameof(layerName));
        }

        Id = id ?? Guid.NewGuid();
        LayerName = layerName;
        this.dimensionBindings = dimensionBindings?.ToArray() ?? Array.Empty<EntityDimensionBinding>();

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Entity id cannot be empty.", nameof(id));
        }

        if (this.dimensionBindings.Any(binding => binding is null))
        {
            throw new ArgumentException("Entity dimension bindings cannot contain null values.", nameof(dimensionBindings));
        }
    }

    /// <summary>
    /// Stabilny identyfikator encji w szkicu.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Nazwa warstwy, do ktorej nalezy encja.
    /// W MVP nazwa warstwy pelni role identyfikatora warstwy.
    /// </summary>
    public string LayerName { get; }

    /// <summary>
    /// Powiazania wymiarow encji z parametrami dokumentu.
    /// </summary>
    public IReadOnlyList<EntityDimensionBinding> DimensionBindings => dimensionBindings;

    /// <summary>
    /// Bounding box encji w milimetrach domenowych.
    /// </summary>
    public abstract BoundingBox Bounds { get; }

    /// <summary>
    /// Zwraca nowa encje po zastosowaniu transformacji afinicznej.
    /// </summary>
    public abstract ISketchEntity Transform(Matrix3x3 transform);

    /// <summary>
    /// Zwraca kopie encji z nowym identyfikatorem.
    /// Uzywaj przy operacjach duplikowania w szkicu.
    /// </summary>
    public abstract Entity Copy(Guid? id = null);
}
