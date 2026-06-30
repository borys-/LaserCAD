using LaserCad.Core.Documents;

namespace LaserCad.Core.Generators;

/// <summary>
/// Wspolny kontrakt generatora, ktory tworzy szkic 2D gotowy do dodania do dokumentu CAD.
/// </summary>
public interface ISketchGenerator
{
    /// <summary>
    /// Nazwa generatora pokazywana w UI i historii modelu.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Generuje szkic wynikowy dla aktualnych opcji generatora.
    /// </summary>
    Sketch GenerateSketch();
}
