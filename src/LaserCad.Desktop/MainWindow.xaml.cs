using System.Windows;
using LaserCad.Core.Documents;
using LaserCad.Core.BoxGenerators;
using LaserCad.Geometry.Units;
using LaserCad.ViewportContract;
using Microsoft.Win32;

namespace LaserCad.Desktop;

/// <summary>
/// Glowne okno aplikacji desktopowej Laser CAD.
/// </summary>
public partial class MainWindow : Window
{
    private readonly DesktopShellViewModel viewModel = new();
    private readonly ViewportIpcClient viewportIpcClient = new();
    private readonly ViewportProcessController viewportProcessController = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = viewModel;
        MaterialProfileComboBox.ItemsSource = viewModel.MaterialProfiles;
        MaterialProfileComboBox.SelectedItem = viewModel.SelectedMaterialProfile;
        RefreshDocumentSummary();
    }

    private void RebuildBoxButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var options = new BoxGeneratorOptions(
                Length.FromMillimeters(ParseMillimeters(BoxWidthTextBox.Text, "Szerokosc")),
                Length.FromMillimeters(ParseMillimeters(BoxDepthTextBox.Text, "Glebokosc")),
                Length.FromMillimeters(ParseMillimeters(BoxHeightTextBox.Text, "Wysokosc")),
                Length.FromMillimeters(ParseMillimeters(MaterialThicknessTextBox.Text, "Grubosc materialu")),
                Length.FromMillimeters(ParseMillimeters(KerfTextBox.Text, "Kerf")),
                Length.FromMillimeters(ParseMillimeters(FingerWidthTextBox.Text, "Szerokosc palca")),
                Length.FromMillimeters(ParseMillimeters(ClearanceTextBox.Text, "Clearance")));

            viewModel.ApplyBoxOptions(options);
            viewportIpcClient.SendDocument(viewModel.CurrentDocument);
            RefreshDocumentSummary();
        }
        catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void ExportSvg_Click(object sender, RoutedEventArgs e)
    {
        ExportFile("SVG files (*.svg)|*.svg", "laser-cad-box.svg", viewModel.ExportSvg);
    }

    private void ExportDxf_Click(object sender, RoutedEventArgs e)
    {
        ExportFile("DXF files (*.dxf)|*.dxf", "laser-cad-box.dxf", viewModel.ExportDxf);
    }

    private void MaterialProfileComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (MaterialProfileComboBox.SelectedItem is not MaterialProfile materialProfile)
        {
            return;
        }

        viewModel.SetMaterialProfile(materialProfile);
        RefreshDocumentSummary();
    }

    private void ResetView_Click(object sender, RoutedEventArgs e)
    {
        viewportIpcClient.SendViewCommand(ViewportViewCommand.ResetView);
        StatusTextBlock.Text = "Wyslano reset widoku do viewportu";
    }

    private void ZoomToFit_Click(object sender, RoutedEventArgs e)
    {
        viewportIpcClient.SendViewCommand(ViewportViewCommand.ZoomToFit);
        StatusTextBlock.Text = "Wyslano zoom to fit do viewportu";
    }

    private void Grid_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not System.Windows.Controls.MenuItem menuItem)
        {
            return;
        }

        viewportIpcClient.SendViewCommand(ViewportViewCommand.SetGridVisibility, menuItem.IsChecked);
        StatusTextBlock.Text = menuItem.IsChecked ? "Wlaczono grid viewportu" : "Wylaczono grid viewportu";
    }

    private void RefreshSelection_Click(object sender, RoutedEventArgs e)
    {
        var selection = viewportIpcClient.ReadLatestSelectionChanged();
        SelectedEntitiesTextBlock.Text = "Zaznaczone: " + (selection?.EntityIds.Count ?? 0);
        StatusTextBlock.Text = selection == null
            ? "Brak zdarzen zaznaczenia z viewportu"
            : "Odczytano zaznaczenie z viewportu";
    }

    private void StartViewport_Click(object sender, RoutedEventArgs e)
    {
        StatusTextBlock.Text = viewportProcessController.TryStart()
            ? "Uruchomiono proces Unity viewport"
            : "Nie znaleziono viewportu albo proces juz dziala: " + viewportProcessController.ViewportExecutablePath;
    }

    private void RestartViewport_Click(object sender, RoutedEventArgs e)
    {
        StatusTextBlock.Text = viewportProcessController.Restart()
            ? "Zrestartowano proces Unity viewport"
            : "Nie znaleziono viewportu: " + viewportProcessController.ViewportExecutablePath;
    }

    private void StopViewport_Click(object sender, RoutedEventArgs e)
    {
        StatusTextBlock.Text = viewportProcessController.Stop()
            ? "Zamknieto proces Unity viewport"
            : "Proces Unity viewport nie byl uruchomiony";
    }

    protected override void OnClosed(EventArgs e)
    {
        viewportProcessController.Dispose();
        base.OnClosed(e);
    }

    private void ExportFile(string filter, string fileName, Func<string, string> export)
    {
        var dialog = new SaveFileDialog
        {
            Filter = filter,
            FileName = fileName,
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        export(dialog.FileName);
        RefreshDocumentSummary();
    }

    private static double ParseMillimeters(string value, string fieldName)
    {
        if (!double.TryParse(value, out var result))
        {
            throw new FormatException("Niepoprawna wartosc pola: " + fieldName);
        }

        return result;
    }

    private void RefreshDocumentSummary()
    {
        DocumentNameTextBlock.Text = "Dokument: " + viewModel.CurrentDocument.Name;
        SketchCountTextBlock.Text = "Szkice: " + viewModel.CurrentDocument.Sketches.Count;
        StatusTextBlock.Text = viewModel.StatusText;
        LayersItemsControl.ItemsSource = viewModel.CurrentDocument.Layers;
    }
}
