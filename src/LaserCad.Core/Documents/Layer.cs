namespace LaserCad.Core.Documents;

public sealed class Layer
{
    public Layer(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Layer name cannot be empty.", nameof(name));
        }

        Name = name;
    }

    public string Name { get; }
}
