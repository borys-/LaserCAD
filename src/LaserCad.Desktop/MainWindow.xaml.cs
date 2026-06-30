using System.Windows;
using LaserCad.Core.BoxGenerators;
using LaserCad.Geometry.Units;

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
    }
}
