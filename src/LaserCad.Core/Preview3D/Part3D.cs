namespace LaserCad.Core.Preview3D;

/// <summary>
/// Pojedyncza czesc 3D utworzona z konturu 2D.
/// </summary>
public sealed class Part3D
{
    /// <summary>
    /// Tworzy czesc z nazwy, mesha i grubosci materialu.
    /// </summary>
    public Part3D(string name, Mesh3D mesh, double thicknessMillimeters)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Part name cannot be empty.", nameof(name));
        }

        Mesh = mesh ?? throw new ArgumentNullException(nameof(mesh));

        if (thicknessMillimeters <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(thicknessMillimeters), "Thickness must be positive.");
        }

        Name = name;
        ThicknessMillimeters = thicknessMillimeters;
    }

    /// <summary>
    /// Nazwa czesci.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Mesh czesci.
    /// </summary>
    public Mesh3D Mesh { get; }

    /// <summary>
    /// Grubosc materialu uzyta do ekstrudowania.
    /// </summary>
    public double ThicknessMillimeters { get; }
}
