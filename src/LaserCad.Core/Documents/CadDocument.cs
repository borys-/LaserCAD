using LaserCad.Core.Parameters;

namespace LaserCad.Core.Documents;

/// <summary>
/// Reprezentuje glowny dokument CAD: metadane projektu, parametry, warstwy, szkice, generatory i profil materialu.
/// Tworz nowa instancje dla pustego projektu albo uzywaj metod Add*/With*, aby otrzymac nowa wersje dokumentu bez mutowania poprzedniej.
/// </summary>
public sealed class CadDocument
{
    /// <summary>
    /// Tworzy dokument CAD z opcjonalnie podanymi kolekcjami domenowymi.
    /// Uzywaj parametrow opcjonalnych przy odtwarzaniu dokumentu z pliku albo zostaw domyslne wartosci dla nowego projektu.
    /// </summary>
    public CadDocument(
        Guid? id = null,
        string name = "Untitled",
        int formatVersion = 1,
        ParameterSet? parameters = null,
        IEnumerable<Layer>? layers = null,
        IEnumerable<Sketch>? sketches = null,
        IEnumerable<GeneratorInstance>? generators = null,
        MaterialProfile? materialProfile = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Document name cannot be empty.", nameof(name));
        }

        if (formatVersion <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(formatVersion), "Document format version must be positive.");
        }

        Id = id ?? Guid.NewGuid();
        Name = name;
        FormatVersion = formatVersion;
        Parameters = parameters ?? new ParameterSet();
        Layers = layers?.ToArray() ?? DefaultLayers.All.ToArray();
        Sketches = sketches?.ToArray() ?? Array.Empty<Sketch>();
        Generators = generators?.ToArray() ?? Array.Empty<GeneratorInstance>();
        MaterialProfile = materialProfile;

        if (Layers.Any(layer => layer is null))
        {
            throw new ArgumentException("Document layers cannot contain null values.", nameof(layers));
        }

        if (Sketches.Any(sketch => sketch is null))
        {
            throw new ArgumentException("Document sketches cannot contain null values.", nameof(sketches));
        }

        if (Generators.Any(generator => generator is null))
        {
            throw new ArgumentException("Document generators cannot contain null values.", nameof(generators));
        }

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Document id cannot be empty.", nameof(id));
        }
    }

    /// <summary>
    /// Stabilny identyfikator dokumentu uzywany przy zapisie, odczycie i porownywaniu projektow.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Nazwa dokumentu prezentowana uzytkownikowi w UI i w metadanych projektu.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Wersja formatu pliku, ktora pozwala przyszlemu serializerowi rozpoznac zgodnosc zapisu.
    /// </summary>
    public int FormatVersion { get; }

    /// <summary>
    /// Zestaw parametrow dokumentu, np. szerokosc, wysokosc, grubosc materialu albo kerf.
    /// </summary>
    public ParameterSet Parameters { get; }

    /// <summary>
    /// Warstwy dokumentu wykorzystywane pozniej do roli ciecia, grawerowania i eksportu.
    /// </summary>
    public IReadOnlyList<Layer> Layers { get; }

    /// <summary>
    /// Szkice nalezace do dokumentu; kazdy szkic przechowuje wlasne encje.
    /// </summary>
    public IReadOnlyList<Sketch> Sketches { get; }

    /// <summary>
    /// Instancje generatorow parametrycznych zapisane w dokumencie, np. generator pudelka.
    /// </summary>
    public IReadOnlyList<GeneratorInstance> Generators { get; }

    /// <summary>
    /// Opcjonalny profil materialu przypisany do dokumentu.
    /// </summary>
    public MaterialProfile? MaterialProfile { get; }

    /// <summary>
    /// Zwraca nowy dokument z dodanym parametrem.
    /// Uzywaj tej metody zamiast modyfikowania kolekcji bezposrednio, aby zachowac niemutowalny styl modelu.
    /// </summary>
    public CadDocument AddParameter(Parameter parameter)
    {
        return new CadDocument(Id, Name, FormatVersion, Parameters.Add(parameter), Layers, Sketches, Generators, MaterialProfile);
    }

    /// <summary>
    /// Zwraca nowy dokument z dodana warstwa.
    /// Przydatne podczas budowania dokumentu produkcyjnego przed eksportem.
    /// </summary>
    public CadDocument AddLayer(Layer layer)
    {
        ArgumentNullException.ThrowIfNull(layer);

        return new CadDocument(Id, Name, FormatVersion, Parameters, Layers.Append(layer), Sketches, Generators, MaterialProfile);
    }

    /// <summary>
    /// Zwraca nowy dokument z dodanym szkicem.
    /// Uzywaj po utworzeniu szkicu, ktory ma stac sie czescia projektu.
    /// </summary>
    public CadDocument AddSketch(Sketch sketch)
    {
        ArgumentNullException.ThrowIfNull(sketch);

        return new CadDocument(Id, Name, FormatVersion, Parameters, Layers, Sketches.Append(sketch), Generators, MaterialProfile);
    }

    /// <summary>
    /// Zwraca nowy dokument z dodana instancja generatora.
    /// Uzywaj, aby zachowac parametryczny generator w historii dokumentu.
    /// </summary>
    public CadDocument AddGenerator(GeneratorInstance generator)
    {
        ArgumentNullException.ThrowIfNull(generator);

        return new CadDocument(Id, Name, FormatVersion, Parameters, Layers, Sketches, Generators.Append(generator), MaterialProfile);
    }

    /// <summary>
    /// Zwraca nowy dokument z ustawionym profilem materialu.
    /// Uzywaj przy wyborze materialu albo zmianie domyslnych ustawien produkcyjnych.
    /// </summary>
    public CadDocument WithMaterialProfile(MaterialProfile materialProfile)
    {
        ArgumentNullException.ThrowIfNull(materialProfile);

        return new CadDocument(Id, Name, FormatVersion, Parameters, Layers, Sketches, Generators, materialProfile);
    }
}
