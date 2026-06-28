namespace LaserCad.Core.Documents;

/// <summary>
/// Reprezentuje warstwe dokumentu CAD.
/// W tej fazie przechowuje nazwe, a role produkcyjne i kolory beda dopinane w kolejnych taskach.
/// </summary>
public sealed class Layer
{
    /// <summary>
    /// Tworzy warstwe o podanej nazwie.
    /// Uzywaj nazw zrozumialych dla uzytkownika, np. "Cut" albo "Engrave".
    /// </summary>
    public Layer(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Layer name cannot be empty.", nameof(name));
        }

        Name = name;
    }

    /// <summary>
    /// Nazwa warstwy wyswietlana w dokumencie i pozniej uzywana przy eksporcie.
    /// </summary>
    public string Name { get; }
}
