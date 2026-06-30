using System.Windows;
using LaserCad.Core.Documents;
using LaserCad.Core.BoxGenerators;
using LaserCad.Geometry.Units;
using Microsoft.Win32;

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
