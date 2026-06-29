using LaserCad.Geometry;

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

    /// <summary>
    /// Zwraca nowy szkic bez encji o podanym identyfikatorze.
    /// Jesli encja nie istnieje, zwracany szkic ma te sama liste encji.
    /// </summary>
    public Sketch RemoveEntity(Guid entityId)
    {
        return new Sketch(Id, Name, Entities.Where(entity => entity.Id != entityId));
    }

    /// <summary>
    /// Zwraca nowy szkic z kopia encji o podanym identyfikatorze.
    /// Kopia otrzymuje nowy identyfikator, chyba ze zostanie jawnie przekazany.
    /// </summary>
    public Sketch CopyEntity(Guid entityId, Guid? copiedEntityId = null)
    {
        Entity entity = FindEntity(entityId);

        return AddEntity(entity.Copy(copiedEntityId));
    }

    /// <summary>
    /// Zwraca nowy szkic z transformowana encja o podanym identyfikatorze.
    /// </summary>
    public Sketch TransformEntity(Guid entityId, Matrix3x3 transform)
    {
        return new Sketch(
            Id,
            Name,
            Entities.Select(entity => entity.Id == entityId ? TransformEntityCore(entity, transform) : entity));
    }

    /// <summary>
    /// Zwraca nowy szkic z encja przesunieta o podany wektor.
    /// </summary>
    public Sketch MoveEntity(Guid entityId, double offsetX, double offsetY)
    {
        return TransformEntity(entityId, Matrix3x3.CreateTranslation(offsetX, offsetY));
    }

    /// <summary>
    /// Zwraca nowy szkic z encja obrocona wokol poczatku ukladu wspolrzednych.
    /// </summary>
    public Sketch RotateEntity(Guid entityId, double angleRadians)
    {
        return TransformEntity(entityId, Matrix3x3.CreateRotation(angleRadians));
    }

    /// <summary>
    /// Zwraca nowy szkic z encja przeskalowana wzgledem poczatku ukladu wspolrzednych.
    /// </summary>
    public Sketch ScaleEntity(Guid entityId, double scaleX, double scaleY)
    {
        return TransformEntity(entityId, Matrix3x3.CreateScaling(scaleX, scaleY));
    }

    /// <summary>
    /// Zwraca nowy szkic z encja odbita wzgledem wybranej osi ukladu wspolrzednych.
    /// </summary>
    public Sketch MirrorEntity(Guid entityId, SketchMirrorAxis axis)
    {
        Matrix3x3 transform = axis switch
        {
            SketchMirrorAxis.X => Matrix3x3.CreateReflectionX(),
            SketchMirrorAxis.Y => Matrix3x3.CreateReflectionY(),
            _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, "Unsupported mirror axis."),
        };

        return TransformEntity(entityId, transform);
    }

    private Entity FindEntity(Guid entityId)
    {
        return Entities.FirstOrDefault(entity => entity.Id == entityId)
            ?? throw new InvalidOperationException($"Sketch entity '{entityId}' was not found.");
    }

    private static Entity TransformEntityCore(Entity entity, Matrix3x3 transform)
    {
        return entity.Transform(transform) as Entity
            ?? throw new InvalidOperationException("Sketch entity transform must return an Entity instance.");
    }
}
