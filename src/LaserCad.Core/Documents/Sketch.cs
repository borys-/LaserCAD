namespace LaserCad.Core.Documents;

/// <summary>
/// Reprezentuje szkic 2D w dokumencie CAD.
/// Szkic grupuje encje geometryczne i moze byc dodany do dokumentu przez CadDocument.AddSketch.
/// </summary>
public sealed class Sketch
{
    /// <summary>
    /// Tworzy szkic z opcjonalnym identyfikatorem, nazwa i poczatkowa lista encji.
    /// Uzywaj domyslnych wartosci dla nowego szkicu albo podawaj wartosci przy odczycie projektu.
    /// </summary>
    public Sketch(Guid? id = null, string name = "Sketch", IEnumerable<Entity>? entities = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Sketch name cannot be empty.", nameof(name));
        }

        Id = id ?? Guid.NewGuid();
        Name = name;
        Entities = entities?.ToArray() ?? Array.Empty<Entity>();

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Sketch id cannot be empty.", nameof(id));
        }

        if (Entities.Any(entity => entity is null))
        {
            throw new ArgumentException("Sketch entities cannot contain null values.", nameof(entities));
        }
    }

    /// <summary>
    /// Stabilny identyfikator szkicu w obrebie dokumentu.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Nazwa szkicu wyswietlana uzytkownikowi.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Encje nalezace do szkicu, np. przyszle linie, prostokaty albo okregi.
    /// </summary>
    public IReadOnlyList<Entity> Entities { get; }

    /// <summary>
    /// Zwraca nowy szkic z dodana encja.
    /// Uzywaj tej metody, aby zachowac niemutowalny styl modelu domenowego.
    /// </summary>
    public Sketch AddEntity(Entity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        return new Sketch(Id, Name, Entities.Append(entity));
    }
}
