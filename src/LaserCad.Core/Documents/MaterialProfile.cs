namespace LaserCad.Core.Documents;

public sealed class MaterialProfile
{
    public MaterialProfile(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Material profile name cannot be empty.", nameof(name));
        }

        Name = name;
    }

    public string Name { get; }
}
