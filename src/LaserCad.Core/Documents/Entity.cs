namespace LaserCad.Core.Documents;

/// <summary>
/// Bazowy kontrakt encji szkicu.
/// Dziedzicz po tym typie przy implementowaniu konkretnych encji, takich jak linia, prostokat albo okrag.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// Tworzy encje z opcjonalnym identyfikatorem.
    /// Konstruktor klas pochodnych powinien przekazac id przy odczycie dokumentu albo zostawic null dla nowej encji.
    /// </summary>
    protected Entity(Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Entity id cannot be empty.", nameof(id));
        }
    }

    /// <summary>
    /// Stabilny identyfikator encji w szkicu.
    /// </summary>
    public Guid Id { get; }
}
