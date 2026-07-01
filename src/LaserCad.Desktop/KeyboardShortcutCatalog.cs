namespace LaserCad.Desktop;

/// <summary>
/// Domyslna konfiguracja skrotow klawiszowych desktop shell.
/// </summary>
public static class KeyboardShortcutCatalog
{
    public static IReadOnlyList<KeyboardShortcut> Defaults { get; } =
    [
        new("Nowy projekt", "Ctrl+N"),
        new("Otworz projekt", "Ctrl+O"),
        new("Zapisz projekt", "Ctrl+S"),
        new("Eksport SVG", "Ctrl+Shift+S"),
        new("Eksport DXF", "Ctrl+Shift+D"),
        new("Undo", "Ctrl+Z"),
        new("Redo", "Ctrl+Y"),
        new("Reset widoku", "Home"),
        new("Zoom to fit", "Ctrl+0"),
        new("Ustawienia", "Ctrl+,"),
    ];
}
