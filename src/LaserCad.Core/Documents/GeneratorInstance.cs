using LaserCad.Core.Parameters;

namespace LaserCad.Core.Documents;

/// <summary>
/// Reprezentuje zapisana w dokumencie instancje generatora parametrycznego.
/// Przechowuje typ generatora i jego parametry, aby model mogl byc przebudowany po zmianie wartosci.
/// </summary>
public sealed class GeneratorInstance
{
    /// <summary>
    /// Tworzy instancje generatora.
    /// Uzywaj generatorType do rozpoznania konkretnego generatora, np. "Box", a parameters do zapisania jego konfiguracji.
    /// </summary>
    public GeneratorInstance(Guid? id = null, string generatorType = "Generator", ParameterSet? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(generatorType))
        {
            throw new ArgumentException("Generator type cannot be empty.", nameof(generatorType));
        }

        Id = id ?? Guid.NewGuid();
        GeneratorType = generatorType;
        Parameters = parameters ?? new ParameterSet();

        if (Id == Guid.Empty)
        {
            throw new ArgumentException("Generator id cannot be empty.", nameof(id));
        }
    }

    /// <summary>
    /// Stabilny identyfikator instancji generatora.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Nazwa lub klucz typu generatora, ktory umozliwia znalezienie implementacji przebudowy.
    /// </summary>
    public string GeneratorType { get; }

    /// <summary>
    /// Parametry przypisane do tej instancji generatora.
    /// </summary>
    public ParameterSet Parameters { get; }

    /// <summary>
    /// Zwraca nowa instancje generatora z dodanym parametrem.
    /// Uzywaj przy konfigurowaniu generatora bez mutowania poprzedniej instancji.
    /// </summary>
    public GeneratorInstance AddParameter(Parameter parameter)
    {
        return new GeneratorInstance(Id, GeneratorType, Parameters.Add(parameter));
    }
}
