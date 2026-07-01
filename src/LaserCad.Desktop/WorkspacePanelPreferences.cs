using System.IO;
using System.Text.Json;

namespace LaserCad.Desktop;

/// <summary>
/// Preferencje widocznosci paneli przestrzeni roboczej.
/// </summary>
public sealed class WorkspacePanelPreferences
{
    public bool IsBoxGeneratorPanelVisible { get; set; }

    public bool IsTemplateLibraryPanelVisible { get; set; }

    public bool AreAdvancedPanelsVisible { get; set; }

    public bool IsHistoryPanelVisible { get; set; }

    public static string DefaultPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "LaserCad",
        "desktop-shell-settings.json");

    public static WorkspacePanelPreferences Load(string? path = null)
    {
        var settingsPath = path ?? DefaultPath;
        if (!File.Exists(settingsPath))
        {
            return new WorkspacePanelPreferences();
        }

        try
        {
            return JsonSerializer.Deserialize<WorkspacePanelPreferences>(File.ReadAllText(settingsPath))
                ?? new WorkspacePanelPreferences();
        }
        catch (JsonException)
        {
            return new WorkspacePanelPreferences();
        }
    }

    public void Save(string? path = null)
    {
        var settingsPath = path ?? DefaultPath;
        var directory = Path.GetDirectoryName(settingsPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(
            settingsPath,
            JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
    }
}
