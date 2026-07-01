using System.Windows;

namespace LaserCad.Desktop;

/// <summary>
/// Proste okno ustawien aplikacji desktopowej.
/// </summary>
public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        KeyboardShortcutsItemsControl.ItemsSource = KeyboardShortcutCatalog.Defaults;
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
