using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using LaserCad.Core.Documents;
using LaserCad.Core.BoxGenerators;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;
using LaserCad.ViewportContract;
using Microsoft.Win32;
using DockStyle = System.Windows.Forms.DockStyle;
using Panel = System.Windows.Forms.Panel;

namespace LaserCad.Desktop;

/// <summary>
/// Glowne okno aplikacji desktopowej Laser CAD.
/// </summary>
public partial class MainWindow : Window
{
    private const int WmMouseWheel = 0x020A;

    private readonly DesktopShellViewModel viewModel = new();
    private readonly ViewportIpcClient viewportIpcClient = new();
    private readonly ViewportProcessController viewportProcessController = new();
    private readonly Panel viewportPanel = new() { Dock = DockStyle.Fill };
    private readonly DispatcherTimer viewportInboxTimer = new() { Interval = TimeSpan.FromMilliseconds(150) };

    public MainWindow()
    {
        InitializeComponent();
        DataContext = viewModel;
        MaterialProfileComboBox.ItemsSource = viewModel.MaterialProfiles;
        MaterialProfileComboBox.SelectedItem = viewModel.SelectedMaterialProfile;
        ViewportHost.Child = viewportPanel;
        viewportPanel.Resize += (_, _) => viewportProcessController.ResizeEmbeddedViewport();
        viewportPanel.MouseEnter += (_, _) => viewportProcessController.FocusViewport();
        viewportPanel.MouseDown += (_, _) => viewportProcessController.FocusViewport();
        ViewportHost.MouseEnter += (_, _) => viewportProcessController.FocusViewport();
        viewportInboxTimer.Tick += (_, _) => ProcessViewportInbox();
        RefreshDocumentSummary();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        AddWindowMessageHook();
        StartEmbeddedViewport();
        viewportInboxTimer.Start();
    }

    private void Window_Activated(object? sender, EventArgs e)
    {
        RestoreViewportInput();
    }

    private void Window_StateChanged(object? sender, EventArgs e)
    {
        if (WindowState != WindowState.Minimized)
        {
            RestoreViewportInput();
        }
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

    private void AddRectangle_Click(object sender, RoutedEventArgs e)
    {
        SetDrawingTool(ViewportDrawingTool.Rectangle);
    }

    private void AddLine_Click(object sender, RoutedEventArgs e)
    {
        SetDrawingTool(ViewportDrawingTool.Line);
    }

    private void AddCircle_Click(object sender, RoutedEventArgs e)
    {
        SetDrawingTool(ViewportDrawingTool.Circle);
    }

    private void SelectTool_Click(object sender, RoutedEventArgs e)
    {
        SetDrawingTool(ViewportDrawingTool.None);
    }

    private void DeleteSelected_Click(object sender, RoutedEventArgs e)
    {
        viewModel.DeleteEntities(ReadSelectedEntityIds());
        PublishDocument();
    }

    private void MoveSelected_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            viewModel.MoveEntities(
                ReadSelectedEntityIds(),
                ParseMillimeters(MoveXTextBox.Text, "Przesuniecie X"),
                ParseMillimeters(MoveYTextBox.Text, "Przesuniecie Y"));
            PublishDocument();
        }
        catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void RotateSelected_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            viewModel.RotateEntities(
                ReadSelectedEntityIds(),
                ParseNumber(RotateDegreesTextBox.Text, "Obrot"));
            PublishDocument();
        }
        catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void ScaleSelected_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            viewModel.ScaleEntities(
                ReadSelectedEntityIds(),
                ParseNumber(ScaleXTextBox.Text, "Skala X"),
                ParseNumber(ScaleYTextBox.Text, "Skala Y"));
            PublishDocument();
        }
        catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void Undo_Click(object sender, RoutedEventArgs e)
    {
        viewModel.Undo();
        PublishDocument();
    }

    private void Redo_Click(object sender, RoutedEventArgs e)
    {
        viewModel.Redo();
        PublishDocument();
    }

    private void MaterialProfileComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (MaterialProfileComboBox.SelectedItem is not MaterialProfile materialProfile)
        {
            return;
        }

        viewModel.SetMaterialProfile(materialProfile);
        viewportIpcClient.SendDocument(viewModel.CurrentDocument);
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

    private void StartEmbeddedViewport()
    {
        ShowViewportLoading("Uruchamianie widoku roboczego", "Ladowanie viewportu Unity...", true);

        if (viewportProcessController.TryStartEmbedded(viewportPanel.Handle))
        {
            ViewportLoadingOverlay.Visibility = Visibility.Collapsed;
            viewportIpcClient.SendDocument(viewModel.CurrentDocument);
            StatusTextBlock.Text = "Gotowe";
            return;
        }

        ShowViewportLoading("Nie znaleziono widoku roboczego", "Sprawdz build viewportu w katalogu aplikacji.", false);
        StatusTextBlock.Text = "Nie znaleziono viewportu: " + viewportProcessController.ViewportExecutablePath;
    }

    private void ShowViewportLoading(string title, string detail, bool isProgressVisible)
    {
        ViewportLoadingOverlay.Visibility = Visibility.Visible;
        ViewportLoadingTitleTextBlock.Text = title;
        ViewportLoadingDetailTextBlock.Text = detail;
        ViewportLoadingProgressBar.Visibility = isProgressVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    private void RestoreViewportInput()
    {
        Dispatcher.BeginInvoke(
            () =>
            {
                viewportProcessController.ResizeEmbeddedViewport();
                viewportProcessController.FocusViewport();
            },
            DispatcherPriority.ApplicationIdle);
    }

    private void AddWindowMessageHook()
    {
        var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
        source?.AddHook(WindowMessageHook);
    }

    private IntPtr WindowMessageHook(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (message == WmMouseWheel && IsMouseOverViewportPanel())
        {
            handled = viewportProcessController.ForwardMouseWheel(wParam, lParam);
        }

        return IntPtr.Zero;
    }

    private bool IsMouseOverViewportPanel()
    {
        if (!viewportPanel.IsHandleCreated)
        {
            return false;
        }

        var panelPoint = viewportPanel.PointToClient(System.Windows.Forms.Control.MousePosition);
        return viewportPanel.ClientRectangle.Contains(panelPoint);
    }

    protected override void OnClosed(EventArgs e)
    {
        viewportInboxTimer.Stop();
        viewportProcessController.Dispose();
        base.OnClosed(e);
    }

    private void ExportFile(string filter, string fileName, Func<string, string> export)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
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
        return ParseNumber(value, fieldName);
    }

    private static double ParseNumber(string value, string fieldName)
    {
        if (!double.TryParse(value, out var result))
        {
            throw new FormatException("Niepoprawna wartosc pola: " + fieldName);
        }

        return result;
    }

    private IReadOnlyList<Guid> ReadSelectedEntityIds()
    {
        var selection = viewportIpcClient.ReadLatestSelectionChanged();
        SelectedEntitiesTextBlock.Text = "Zaznaczone: " + (selection?.EntityIds.Count ?? 0);
        return selection?.EntityIds.ToArray() ?? Array.Empty<Guid>();
    }

    private void SetDrawingTool(ViewportDrawingTool tool)
    {
        viewportIpcClient.SendDrawingTool(tool);
        StatusTextBlock.Text = tool == ViewportDrawingTool.None
            ? "Tryb zaznaczania"
            : "Kliknij dwa punkty w viewportcie: " + tool;
    }

    private void ProcessViewportInbox()
    {
        var changed = false;
        foreach (var message in viewportIpcClient.ReadPendingShapeDrawn())
        {
            ApplyDrawnShape(message);
            changed = true;
        }

        if (changed)
        {
            PublishDocument();
        }
    }

    private void ApplyDrawnShape(ViewportShapeDrawnMessage message)
    {
        var start = new Point2D(message.Start.X, message.Start.Y);
        var end = new Point2D(message.End.X, message.End.Y);

        switch (message.Tool)
        {
            case ViewportDrawingTool.Rectangle:
                viewModel.AddRectangle(start, end);
                break;
            case ViewportDrawingTool.Line:
                viewModel.AddLine(start, end);
                break;
            case ViewportDrawingTool.Circle:
                viewModel.AddCircle(start, end);
                break;
        }
    }

    private void PublishDocument()
    {
        viewportIpcClient.SendDocument(viewModel.CurrentDocument);
        RefreshDocumentSummary();
    }

    private void RefreshDocumentSummary()
    {
        DocumentNameTextBlock.Text = "Dokument: " + viewModel.CurrentDocument.Name;
        SketchCountTextBlock.Text = "Szkice: " + viewModel.CurrentDocument.Sketches.Count;
        UndoCountTextBlock.Text = "Undo: " + viewModel.UndoCount;
        RedoCountTextBlock.Text = "Redo: " + viewModel.RedoCount;
        StatusTextBlock.Text = viewModel.StatusText;
        LayersItemsControl.ItemsSource = viewModel.CurrentDocument.Layers;
    }
}
