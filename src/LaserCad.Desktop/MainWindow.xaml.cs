using System.Windows;

namespace LaserCad.Desktop;

/// <summary>
/// Glowne okno aplikacji desktopowej Laser CAD.
/// </summary>
public partial class MainWindow : Window
{
    private readonly DesktopShellViewModel viewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
