namespace LaserCad.Core.Documents;

/// <summary>
/// Reprezentuje warstwe dokumentu CAD.
/// Przechowuje nazwe, kolor i role uzywane pozniej przez eksport oraz UI.
/// </summary>
public sealed class Layer
{
    /// <summary>
    /// Tworzy warstwe o podanej nazwie.
    /// Uzywaj nazw zrozumialych dla uzytkownika, np. "Cut" albo "Engrave".
    /// </summary>
    public Layer(string name, LayerColor? color = null, LayerRole role = LayerRole.Cut)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Layer name cannot be empty.", nameof(name));
        }

        Name = name;
        Color = color ?? LayerColor.Black;
        Role = role;
    }

    /// <summary>
    /// Nazwa warstwy wyswietlana w dokumencie i pozniej uzywana przy eksporcie.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Kolor warstwy uzywany do prezentacji i eksportu.
    /// </summary>
    public LayerColor Color { get; }

    /// <summary>
    /// Produkcyjna rola warstwy, np. ciecie, grawerowanie albo ignorowanie.
    /// </summary>
    public LayerRole Role { get; }
}
