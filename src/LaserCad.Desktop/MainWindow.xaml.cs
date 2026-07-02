using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using LaserCad.Core.BoxGenerators;
using LaserCad.Core.Documents;
using LaserCad.Core.Kerf;
using LaserCad.Core.Library;
using LaserCad.Core.MaterialModel;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;
using LaserCad.ViewportContract;
using Microsoft.Win32;
using DockStyle = System.Windows.Forms.DockStyle;
using Panel = System.Windows.Forms.Panel;
using WinFormsCursor = System.Windows.Forms.Cursor;
using WinFormsCursors = System.Windows.Forms.Cursors;

namespace LaserCad.Desktop;

/// <summary>
/// Glowne okno aplikacji desktopowej Laser CAD.
/// </summary>
public partial class MainWindow : Window
{
    private const int WmMouseWheel = 0x020A;

    private static readonly RoutedCommand NewProjectCommand = new();
    private static readonly RoutedCommand OpenProjectCommand = new();
    private static readonly RoutedCommand SaveProjectCommand = new();
    private static readonly RoutedCommand ExportSvgCommand = new();
    private static readonly RoutedCommand ExportDxfCommand = new();
    private static readonly RoutedCommand UndoCommand = new();
    private static readonly RoutedCommand RedoCommand = new();
    private static readonly RoutedCommand ResetViewCommand = new();
    private static readonly RoutedCommand ZoomToFitCommand = new();
    private static readonly RoutedCommand SettingsCommand = new();

    private readonly DesktopShellViewModel viewModel = new();
    private readonly ViewportIpcClient viewportIpcClient = new();
    private readonly ViewportProcessController viewportProcessController = new();
    private readonly WinFormsCursor hiddenViewportCursor = CreateHiddenCursor();
    private readonly Panel viewportPanel = new() { Dock = DockStyle.Fill };
    private readonly DispatcherTimer viewportInboxTimer = new() { Interval = TimeSpan.FromMilliseconds(150) };
    private WorkspacePanelPreferences workspacePanelPreferences = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = viewModel;
        MaterialProfileComboBox.ItemsSource = viewModel.MaterialProfiles;
        MaterialProfileComboBox.SelectedItem = viewModel.SelectedMaterialProfile;
        LibraryTemplateComboBox.ItemsSource = viewModel.LibraryTemplates;
        LibraryTemplateComboBox.SelectedItem = viewModel.SelectedLibraryTemplate;
        ViewportHost.Child = viewportPanel;
        viewportPanel.Resize += (_, _) => viewportProcessController.ResizeEmbeddedViewport();
        viewportPanel.MouseEnter += (_, _) => viewportProcessController.FocusViewport();
        viewportPanel.MouseDown += (_, _) => viewportProcessController.FocusViewport();
        ViewportHost.MouseEnter += (_, _) => viewportProcessController.FocusViewport();
        viewportInboxTimer.Tick += (_, _) => ProcessViewportInbox();
        ConfigureKeyboardShortcuts();
        LoadWorkspacePanelPreferences();
        RefreshDocumentSummary();
    }

    private void ConfigureKeyboardShortcuts()
    {
        AddShortcut(NewProjectCommand, Key.N, ModifierKeys.Control, NewProject_Click);
        AddShortcut(OpenProjectCommand, Key.O, ModifierKeys.Control, OpenProject_Click);
        AddShortcut(SaveProjectCommand, Key.S, ModifierKeys.Control, SaveProject_Click);
        AddShortcut(ExportSvgCommand, Key.S, ModifierKeys.Control | ModifierKeys.Shift, ExportSvg_Click);
        AddShortcut(ExportDxfCommand, Key.D, ModifierKeys.Control | ModifierKeys.Shift, ExportDxf_Click);
        AddShortcut(UndoCommand, Key.Z, ModifierKeys.Control, Undo_Click);
        AddShortcut(RedoCommand, Key.Y, ModifierKeys.Control, Redo_Click);
        AddShortcut(ResetViewCommand, Key.Home, ModifierKeys.None, ResetView_Click);
        AddShortcut(ZoomToFitCommand, Key.D0, ModifierKeys.Control, ZoomToFit_Click);
        AddShortcut(SettingsCommand, Key.OemComma, ModifierKeys.Control, Settings_Click);
    }

    private void AddShortcut(RoutedCommand command, Key key, ModifierKeys modifiers, RoutedEventHandler handler)
    {
        InputBindings.Add(new KeyBinding(command, new KeyGesture(key, modifiers)));
        CommandBindings.Add(new CommandBinding(command, (_, e) => handler(this, e)));
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

    private void PrepareSheet_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var previewDocument = viewModel.CreateNestingPreviewDocument(
                ParseMillimeters(SheetWidthTextBox.Text, "Szerokosc arkusza"),
                ParseMillimeters(SheetHeightTextBox.Text, "Wysokosc arkusza"),
                ParseMillimeters(SheetMarginTextBox.Text, "Margines arkusza"),
                ParseMillimeters(SheetSpacingTextBox.Text, "Odstep czesci"),
                SheetAllowRotationCheckBox.IsChecked == true);

            SaveWorkflowPreferences();
            viewportIpcClient.SendDocument(previewDocument);
            RefreshDocumentSummary();
        }
        catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException or ArgumentException or InvalidOperationException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void PrepareToCut_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var previewDocument = viewModel.PrepareToCut(
                ParseMillimeters(SheetWidthTextBox.Text, "Szerokosc arkusza"),
                ParseMillimeters(SheetHeightTextBox.Text, "Wysokosc arkusza"),
                ParseMillimeters(SheetMarginTextBox.Text, "Margines arkusza"),
                ParseMillimeters(SheetSpacingTextBox.Text, "Odstep czesci"),
                SheetAllowRotationCheckBox.IsChecked == true);

            SaveWorkflowPreferences();
            viewportIpcClient.SendDocument(previewDocument);
            RefreshDocumentSummary();
        }
        catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException or ArgumentException or InvalidOperationException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void NewProject_Click(object sender, RoutedEventArgs e)
    {
        viewModel.NewDocument();
        MaterialProfileComboBox.SelectedItem = viewModel.SelectedMaterialProfile;
        PublishDocument();
    }

    private void OpenProject_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Laser CAD project (*.lasercad.json)|*.lasercad.json|JSON files (*.json)|*.json|All files (*.*)|*.*",
            FileName = "laser-cad-project.lasercad.json",
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        try
        {
            viewModel.LoadProject(dialog.FileName);
        MaterialProfileComboBox.ItemsSource = viewModel.MaterialProfiles;
        MaterialProfileComboBox.SelectedItem = viewModel.SelectedMaterialProfile;
        LibraryTemplateComboBox.ItemsSource = viewModel.LibraryTemplates;
        LibraryTemplateComboBox.SelectedItem = viewModel.SelectedLibraryTemplate;
        PublishDocument();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException or NotSupportedException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void SaveProject_Click(object sender, RoutedEventArgs e)
    {
        if (viewModel.CurrentProjectPath is null)
        {
            SaveProjectAs_Click(sender, e);
            return;
        }

        try
        {
            viewModel.SaveProject(viewModel.CurrentProjectPath);
            RefreshDocumentSummary();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void SaveProjectAs_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "Laser CAD project (*.lasercad.json)|*.lasercad.json|JSON files (*.json)|*.json|All files (*.*)|*.*",
            FileName = viewModel.CurrentProjectPath is null
                ? "laser-cad-project.lasercad.json"
                : System.IO.Path.GetFileName(viewModel.CurrentProjectPath),
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        try
        {
            viewModel.SaveProject(dialog.FileName);
            RefreshDocumentSummary();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ExportSvg_Click(object sender, RoutedEventArgs e)
    {
        ExportFile("SVG files (*.svg)|*.svg", "laser-cad-box.svg", viewModel.ExportSvg);
    }

    private void ExportDxf_Click(object sender, RoutedEventArgs e)
    {
        ExportFile("DXF files (*.dxf)|*.dxf", "laser-cad-box.dxf", viewModel.ExportDxf);
    }

    private void ExportNestedDxf_Click(object sender, RoutedEventArgs e)
    {
        using var dialog = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "Wybierz katalog eksportu DXF z nestingu",
            UseDescriptionForTitle = true,
        };

        if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        {
            return;
        }

        try
        {
            viewModel.ExportNestedDxf(
                dialog.SelectedPath,
                ParseMillimeters(SheetWidthTextBox.Text, "Szerokosc arkusza"),
                ParseMillimeters(SheetHeightTextBox.Text, "Wysokosc arkusza"),
                ParseMillimeters(SheetMarginTextBox.Text, "Margines arkusza"),
                ParseMillimeters(SheetSpacingTextBox.Text, "Odstep czesci"),
                SheetAllowRotationCheckBox.IsChecked == true);
            RefreshDocumentSummary();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or FormatException or ArgumentOutOfRangeException or ArgumentException or InvalidOperationException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void AddRectangle_Click(object sender, RoutedEventArgs e)
    {
        SetDrawingTool(ViewportDrawingTool.Rectangle);
    }

    private void AddMaterialPlate_Click(object sender, RoutedEventArgs e)
    {
        SetDrawingTool(ViewportDrawingTool.MaterialPlate);
    }

    private void AddSlopedMaterialSolid_Click(object sender, RoutedEventArgs e)
    {
        viewModel.AddDefaultSlopedMaterialSolid();
        PublishSlopedWorkflowPreview();
    }

    private void AddSlopedMaterialSolidFromPanel_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            viewModel.AddSlopedMaterialSolid(
                ReadSlopedConstructionType(),
                ParseMillimeters(SlopedWidthTextBox.Text, "Szerokosc bryly"),
                ParseMillimeters(SlopedDepthTextBox.Text, "Glebokosc bryly"),
                ParseMillimeters(SlopedFrontHeightTextBox.Text, "Wysokosc przodu"),
                ParseMillimeters(SlopedBackHeightTextBox.Text, "Wysokosc tylu"),
                ParseMillimeters(SlopedKerfTextBox.Text, "Kerf bryly"));
            SaveWorkflowPreferences();
            PublishSlopedWorkflowPreview();
        }
        catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException or ArgumentException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void AddCircularCutout_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            viewModel.AddCircularCutoutToLatestSlopedMaterialSolid(
                ReadSlopedCutoutFaceName(),
                ParseMillimeters(SlopedCutoutDiameterTextBox.Text, "Srednica otworu"),
                ParseMillimeters(SlopedCutoutLeftTextBox.Text, "Odleglosc od lewej krawedzi"),
                ParseMillimeters(SlopedCutoutBottomTextBox.Text, "Odleglosc od dolnej krawedzi"));
            SaveWorkflowPreferences();
            PublishSlopedWorkflowPreview();
        }
        catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException or ArgumentException or InvalidOperationException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void CopyCircularCutoutToOppositeFace_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            viewModel.CopyLatestCircularCutoutToOppositeFace();
            PublishSlopedWorkflowPreview();
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            StatusTextBlock.Text = ex.Message;
        }
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
        SaveWorkflowPreferences();
        viewportIpcClient.SendDocument(viewModel.CurrentDocument);
        RefreshDocumentSummary();
    }

    private void LibraryTemplateComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        viewModel.SelectLibraryTemplate(LibraryTemplateComboBox.SelectedItem as LibraryTemplate);
        RefreshDocumentSummary();
    }

    private void ApplyLibraryTemplate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            viewModel.ApplySelectedLibraryTemplate();
            PublishDocument();
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException or ArgumentOutOfRangeException)
        {
            StatusTextBlock.Text = ex.Message;
        }
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

    private void BoxGeneratorPanel_Click(object sender, RoutedEventArgs e)
    {
        ApplyWorkspacePanelVisibility();
        SaveWorkspacePanelPreferences();
    }

    private void TemplateLibraryPanel_Click(object sender, RoutedEventArgs e)
    {
        ApplyWorkspacePanelVisibility();
        SaveWorkspacePanelPreferences();
    }

    private void AdvancedPanels_Click(object sender, RoutedEventArgs e)
    {
        ApplyWorkspacePanelVisibility();
        SaveWorkspacePanelPreferences();
    }

    private void HistoryPanel_Click(object sender, RoutedEventArgs e)
    {
        ApplyWorkspacePanelVisibility();
        SaveWorkspacePanelPreferences();
    }

    private void CleanWorkspace_Click(object sender, RoutedEventArgs e)
    {
        BoxGeneratorPanelMenuItem.IsChecked = false;
        TemplateLibraryPanelMenuItem.IsChecked = false;
        AdvancedPanelsMenuItem.IsChecked = false;
        HistoryPanelMenuItem.IsChecked = false;

        ApplyWorkspacePanelVisibility();
        SaveWorkspacePanelPreferences();

        StatusTextBlock.Text = "Czysty widok roboczy";
    }

    private void LoadWorkspacePanelPreferences()
    {
        workspacePanelPreferences = WorkspacePanelPreferences.Load();
        BoxGeneratorPanelMenuItem.IsChecked = workspacePanelPreferences.IsBoxGeneratorPanelVisible;
        TemplateLibraryPanelMenuItem.IsChecked = workspacePanelPreferences.IsTemplateLibraryPanelVisible;
        AdvancedPanelsMenuItem.IsChecked = workspacePanelPreferences.AreAdvancedPanelsVisible;
        HistoryPanelMenuItem.IsChecked = workspacePanelPreferences.IsHistoryPanelVisible;
        ApplyWorkflowPreferences();
        ApplyWorkspacePanelVisibility();
    }

    private void SaveWorkspacePanelPreferences()
    {
        workspacePanelPreferences.IsBoxGeneratorPanelVisible = BoxGeneratorPanelMenuItem.IsChecked;
        workspacePanelPreferences.IsTemplateLibraryPanelVisible = TemplateLibraryPanelMenuItem.IsChecked;
        workspacePanelPreferences.AreAdvancedPanelsVisible = AdvancedPanelsMenuItem.IsChecked;
        workspacePanelPreferences.IsHistoryPanelVisible = HistoryPanelMenuItem.IsChecked;
        SaveWorkflowPreferencesToModel();
        workspacePanelPreferences.Save();
    }

    private void ApplyWorkflowPreferences()
    {
        if (!string.IsNullOrWhiteSpace(workspacePanelPreferences.LastMaterialProfileName))
        {
            var material = viewModel.MaterialProfiles.FirstOrDefault(profile => profile.Name == workspacePanelPreferences.LastMaterialProfileName);
            if (material is not null)
            {
                MaterialProfileComboBox.SelectedItem = material;
            }
        }

        SheetWidthTextBox.Text = FormatPreferenceNumber(workspacePanelPreferences.LastSheetWidthMillimeters);
        SheetHeightTextBox.Text = FormatPreferenceNumber(workspacePanelPreferences.LastSheetHeightMillimeters);
        SheetMarginTextBox.Text = FormatPreferenceNumber(workspacePanelPreferences.LastSheetMarginMillimeters);
        SheetSpacingTextBox.Text = FormatPreferenceNumber(workspacePanelPreferences.LastSheetSpacingMillimeters);
        SheetAllowRotationCheckBox.IsChecked = workspacePanelPreferences.LastSheetAllowRotation;
        SlopedKerfTextBox.Text = FormatPreferenceNumber(workspacePanelPreferences.LastSlopedKerfMillimeters);
        SlopedCutoutDiameterTextBox.Text = FormatPreferenceNumber(workspacePanelPreferences.LastCutoutDiameterMillimeters);
    }

    private void SaveWorkflowPreferencesToModel()
    {
        workspacePanelPreferences.LastMaterialProfileName = viewModel.SelectedMaterialProfile.Name;
        workspacePanelPreferences.LastSheetWidthMillimeters = TryReadPreferenceNumber(SheetWidthTextBox.Text, workspacePanelPreferences.LastSheetWidthMillimeters);
        workspacePanelPreferences.LastSheetHeightMillimeters = TryReadPreferenceNumber(SheetHeightTextBox.Text, workspacePanelPreferences.LastSheetHeightMillimeters);
        workspacePanelPreferences.LastSheetMarginMillimeters = TryReadPreferenceNumber(SheetMarginTextBox.Text, workspacePanelPreferences.LastSheetMarginMillimeters);
        workspacePanelPreferences.LastSheetSpacingMillimeters = TryReadPreferenceNumber(SheetSpacingTextBox.Text, workspacePanelPreferences.LastSheetSpacingMillimeters);
        workspacePanelPreferences.LastSheetAllowRotation = SheetAllowRotationCheckBox.IsChecked == true;
        workspacePanelPreferences.LastSlopedKerfMillimeters = TryReadPreferenceNumber(SlopedKerfTextBox.Text, workspacePanelPreferences.LastSlopedKerfMillimeters);
        workspacePanelPreferences.LastCutoutDiameterMillimeters = TryReadPreferenceNumber(SlopedCutoutDiameterTextBox.Text, workspacePanelPreferences.LastCutoutDiameterMillimeters);
    }

    private void SaveWorkflowPreferences()
    {
        SaveWorkflowPreferencesToModel();
        workspacePanelPreferences.Save();
    }

    private void ApplyWorkspacePanelVisibility()
    {
        BoxGeneratorPanelGroupBox.Visibility = Visibility.Collapsed;
        TemplateLibraryPanelGroupBox.Visibility = Visibility.Collapsed;
        KerfPanelGroupBox.Visibility = Visibility.Collapsed;
        KerfCalibrationPanelGroupBox.Visibility = Visibility.Collapsed;
        TransformsPanelGroupBox.Visibility = Visibility.Collapsed;
        HistoryPanelGroupBox.Visibility = Visibility.Collapsed;

        if (BoxGeneratorPanelMenuItem.IsChecked)
        {
            BoxGeneratorPanelGroupBox.Visibility = Visibility.Visible;
        }

        if (TemplateLibraryPanelMenuItem.IsChecked)
        {
            TemplateLibraryPanelGroupBox.Visibility = Visibility.Visible;
        }

        if (AdvancedPanelsMenuItem.IsChecked)
        {
            KerfPanelGroupBox.Visibility = Visibility.Visible;
            KerfCalibrationPanelGroupBox.Visibility = Visibility.Visible;
            TransformsPanelGroupBox.Visibility = Visibility.Visible;
        }

        if (HistoryPanelMenuItem.IsChecked)
        {
            HistoryPanelGroupBox.Visibility = Visibility.Visible;
        }
    }

    private void Settings_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow
        {
            Owner = this,
        };
        settingsWindow.ShowDialog();
        StatusTextBlock.Text = "Zamknieto ustawienia";
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        System.Windows.MessageBox.Show(
            this,
            "Laser CAD\nParametryczny CAD do projektow pod wycinarke laserowa.\n\nWersja MVP 1.0",
            "About Laser CAD",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void ShowNominalKerfPreview_Click(object sender, RoutedEventArgs e)
    {
        PublishKerfPreview(afterCompensation: false);
    }

    private void ShowCompensatedKerfPreview_Click(object sender, RoutedEventArgs e)
    {
        PublishKerfPreview(afterCompensation: true);
    }

    private void SaveKerfCalibrationMeasurement_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            viewModel.SetKerfCalibrationMeasurement(
                ParseInteger(KerfCalibrationSlotIndexTextBox.Text, "Szczelina"),
                ParseMillimeters(KerfCalibrationMeasuredWidthTextBox.Text, "Pomiar szczeliny"));
            RefreshDocumentSummary();
        }
        catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private void SaveKerfRecommendationToProfile_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            viewModel.SaveKerfRecommendationToMaterialProfile();
            MaterialProfileComboBox.SelectedItem = viewModel.SelectedMaterialProfile;
            PublishDocument();
        }
        catch (InvalidOperationException ex)
        {
            StatusTextBlock.Text = ex.Message;
        }
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
        hiddenViewportCursor.Dispose();
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

    private static int ParseInteger(string value, string fieldName)
    {
        if (!int.TryParse(value, out var result))
        {
            throw new FormatException("Niepoprawna wartosc pola: " + fieldName);
        }

        return result;
    }

    private static string FormatPreferenceNumber(double value)
    {
        return value.ToString("0.###");
    }

    private static double TryReadPreferenceNumber(string value, double fallback)
    {
        return double.TryParse(value, out var result) ? result : fallback;
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
        viewportPanel.Cursor = tool == ViewportDrawingTool.None ? WinFormsCursors.Default : hiddenViewportCursor;
        StatusTextBlock.Text = tool == ViewportDrawingTool.None
            ? "Tryb zaznaczania"
            : "Kliknij dwa punkty w viewportcie: " + tool;
    }

    private static WinFormsCursor CreateHiddenCursor()
    {
        using var emptyBitmap = new System.Drawing.Bitmap(1, 1);
        return new WinFormsCursor(emptyBitmap.GetHicon());
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
            case ViewportDrawingTool.MaterialPlate:
                viewModel.AddMaterialPlate(start, end);
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

    private void PublishSlopedWorkflowPreview()
    {
        viewportIpcClient.SendDocument(viewModel.CreateSlopedWorkflowPreviewDocument());
        RefreshDocumentSummary();
    }

    private void PublishKerfPreview(bool afterCompensation)
    {
        try
        {
            var options = new KerfCompensationOptions(
                Length.FromMillimeters(ParseMillimeters(KerfTextBox.Text, "Kerf")),
                ReadKerfMode());
            var previewDocument = viewModel.CreateKerfPreviewDocument(options, afterCompensation);
            viewportIpcClient.SendDocument(previewDocument);
            RefreshDocumentSummary();
        }
        catch (Exception ex) when (ex is FormatException or ArgumentOutOfRangeException or ArgumentException or InvalidOperationException)
        {
            StatusTextBlock.Text = ex.Message;
        }
    }

    private KerfCompensationMode ReadKerfMode()
    {
        return KerfModeComboBox.SelectedIndex == 1
            ? KerfCompensationMode.Negative
            : KerfCompensationMode.Positive;
    }

    private string ReadSlopedConstructionType()
    {
        return SlopedConstructionTypeComboBox.SelectedItem is ComboBoxItem item
            ? item.Content?.ToString() ?? "Bryla z pochyla gora"
            : "Bryla z pochyla gora";
    }

    private string ReadSlopedCutoutFaceName()
    {
        return SlopedCutoutFaceComboBox.SelectedItem is ComboBoxItem item
            ? item.Content?.ToString() ?? "Front"
            : "Front";
    }

    private void RefreshDocumentSummary()
    {
        DocumentNameTextBlock.Text = "Dokument: " + viewModel.CurrentDocument.Name;
        SketchCountTextBlock.Text = "Szkice: " + viewModel.CurrentDocument.Sketches.Count;
        MaterialSolidCountTextBlock.Text = "Plyty 3D: " + viewModel.CurrentDocument.MaterialSolids.Count;
        SlopedMaterialSolidCountTextBlock.Text = "Bryly trapezowe: " + viewModel.CurrentDocument.SlopedMaterialSolids.Count;
        SlopedMaterialNameTextBlock.Text = viewModel.SelectedMaterialProfile.Name;
        UndoCountTextBlock.Text = "Undo: " + viewModel.UndoCount;
        RedoCountTextBlock.Text = "Redo: " + viewModel.RedoCount;
        StatusTextBlock.Text = viewModel.StatusText;
        LayersItemsControl.ItemsSource = viewModel.CurrentDocument.Layers;
        KerfCalibrationRecommendationTextBlock.Text = viewModel.KerfCalibrationRecommendedKerfMillimeters is double kerf
            ? $"Rekomendacja: {kerf:0.###} mm"
            : "Rekomendacja: -";
        LibraryTemplateDescriptionTextBlock.Text = viewModel.SelectedLibraryTemplate?.Description ?? "Brak szablonu";
        NestingSummaryTextBlock.Text = viewModel.LastNestedPartCount > 0
            ? $"Nesting: {viewModel.LastNestedPartCount} czesci / {viewModel.LastNestedSheetCount} ark. / {viewModel.LastMaterialUsageRatio:P0} / {viewModel.LastCuttingLengthMillimeters:0.#} mm"
            : "Nesting: -";
        ManufacturingChecksSummaryTextBlock.Text = viewModel.LastManufacturingCheckSummary;
    }
}
