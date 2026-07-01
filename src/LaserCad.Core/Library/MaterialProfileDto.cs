namespace LaserCad.Core.Library;

internal sealed class MaterialProfileDto
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public double ThicknessMillimeters { get; set; }

    public double DefaultKerfMillimeters { get; set; }

    public double DefaultClearanceMillimeters { get; set; }

    public double MinimumFingerWidthMillimeters { get; set; }

    public void Validate(string path)
    {
        if (string.IsNullOrWhiteSpace(Id))
        {
            throw new InvalidOperationException("Profil materialu nie ma identyfikatora: " + path);
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new InvalidOperationException("Profil materialu nie ma nazwy: " + path);
        }

        if (ThicknessMillimeters <= 0.0)
        {
            throw new InvalidOperationException("Profil materialu musi miec dodatnia grubosc: " + path);
        }

        if (DefaultKerfMillimeters < 0.0 || DefaultClearanceMillimeters < 0.0 || MinimumFingerWidthMillimeters < 0.0)
        {
            throw new InvalidOperationException("Profil materialu ma ujemne parametry produkcyjne: " + path);
        }
    }
}
