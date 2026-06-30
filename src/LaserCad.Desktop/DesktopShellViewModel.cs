using System;
using System.Collections.ObjectModel;
using System.IO;
using LaserCad.Core.BoxGenerators;
using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Core.Kerf;
using LaserCad.Export.Dxf;
using LaserCad.Export.Svg;
using LaserCad.Geometry;

namespace LaserCad.Desktop;

/// <summary>
/// Stan i akcje glownego desktop shell.
/// </summary>
public sealed class DesktopShellViewModel
{
    private readonly BoxGenerator boxGenerator = new();
    private UndoRedoStack history;

    public DesktopShellViewModel()
    {
        MaterialProfiles = new ObservableCollection<MaterialProfile>(DefaultMaterialProfiles.All);
        SelectedMaterialProfile = DefaultMaterialProfiles.Plywood3Mm;
        BoxOptions = new BoxGeneratorOptions();
        history = new UndoRedoStack(CreateBoxDocument(BoxOptions, SelectedMaterialProfile));
        StatusText = "Gotowe";
    }

    public CadDocument CurrentDocument => history.CurrentDocument;

    public BoxGeneratorOptions BoxOptions { get; private set; }

    public ObservableCollection<MaterialProfile> MaterialProfiles { get; }

    public MaterialProfile SelectedMaterialProfile { get; private set; }

    public string StatusText { get; private set; }

    public int UndoCount => history.UndoCount;

    public int RedoCount => history.RedoCount;

    public bool CanUndo => history.CanUndo;

    public bool CanRedo => history.CanRedo;

    public int KerfCalibrationSlotIndex { get; private set; }

    public double KerfCalibrationMeasuredWidthMillimeters { get; private set; }

    public double? KerfCalibrationRecommendedKerfMillimeters { get; private set; }

    public void NewDocument()
    {
        ReplaceDocument(new CadDocument(name: "Nowy projekt").WithMaterialProfile(SelectedMaterialProfile));
        StatusText = "Utworzono nowy projekt";
    }

    public void ApplyBoxOptions(BoxGeneratorOptions options)
    {
        BoxOptions = options ?? throw new ArgumentNullException(nameof(options));
        ReplaceDocument(CreateBoxDocument(BoxOptions, SelectedMaterialProfile));
        StatusText = "Przebudowano podglad pudelka";
    }

    public void SetMaterialProfile(MaterialProfile materialProfile)
    {
        SelectedMaterialProfile = materialProfile ?? throw new ArgumentNullException(nameof(materialProfile));
        ReplaceDocument(CurrentDocument.WithMaterialProfile(SelectedMaterialProfile));
        StatusText = "Zmieniono profil materialu";
    }

    public void AddRectangle()
    {
        AddRectangle(new Point2D(0.0, 0.0), new Point2D(40.0, 30.0));
    }

    public void AddRectangle(Point2D start, Point2D end)
    {
        var minX = Math.Min(start.X, end.X);
        var minY = Math.Min(start.Y, end.Y);
        var width = Math.Abs(end.X - start.X);
        var height = Math.Abs(end.Y - start.Y);

        if (width <= 0.0 || height <= 0.0)
        {
            StatusText = "Prostokat wymaga dwoch roznych punktow";
            return;
        }

        Execute(new AddEntityCommand(GetEditableSketchId(), new RectangleEntity(new Point2D(minX, minY), width, height, layerName: "Cut")));
        StatusText = "Dodano prostokat";
    }

    public void AddLine()
    {
        AddLine(new Point2D(0.0, 0.0), new Point2D(60.0, 0.0));
    }

    public void AddLine(Point2D start, Point2D end)
    {
        Execute(new AddEntityCommand(GetEditableSketchId(), new LineEntity(new LineSegment2D(start, end), layerName: "Cut")));
        StatusText = "Dodano linie";
    }

    public void AddCircle()
    {
        AddCircle(new Point2D(20.0, 20.0), new Point2D(35.0, 20.0));
    }

    public void AddCircle(Point2D center, Point2D radiusPoint)
    {
        var radius = center.DistanceTo(radiusPoint);
        if (radius <= 0.0)
        {
            StatusText = "Okrag wymaga promienia wiekszego od zera";
            return;
        }

        Execute(new AddEntityCommand(GetEditableSketchId(), new CircleEntity(new Circle2D(center, radius), layerName: "Engrave")));
        StatusText = "Dodano okrag";
    }

    public void DeleteEntities(IEnumerable<Guid> entityIds)
    {
        var commands = FindEntities(entityIds)
            .Select(item => new DeleteCommand(item.SketchId, item.Entity))
            .Cast<ICommand>()
            .ToArray();

        ExecuteCommands(commands, "Usunieto zaznaczone encje");
    }

    public void MoveEntities(IEnumerable<Guid> entityIds, double offsetX, double offsetY)
    {
        ExecuteCommands(
            FindEntities(entityIds).Select(item => new MoveCommand(item.SketchId, item.Entity.Id, offsetX, offsetY)),
            "Przesunieto zaznaczone encje");
    }

    public void RotateEntities(IEnumerable<Guid> entityIds, double angleDegrees)
    {
        var angleRadians = angleDegrees * Math.PI / 180.0;
        ExecuteCommands(
            FindEntities(entityIds).Select(item => new RotateCommand(item.SketchId, item.Entity.Id, angleRadians)),
            "Obrocono zaznaczone encje");
    }

    public void ScaleEntities(IEnumerable<Guid> entityIds, double scaleX, double scaleY)
    {
        ExecuteCommands(
            FindEntities(entityIds).Select(item => new ScaleCommand(item.SketchId, item.Entity.Id, scaleX, scaleY)),
            "Przeskalowano zaznaczone encje");
    }

    public void Undo()
    {
        if (!history.CanUndo)
        {
            StatusText = "Brak operacji do cofniecia";
            return;
        }

        history.Undo();
        StatusText = "Cofnieto operacje";
    }

    public void Redo()
    {
        if (!history.CanRedo)
        {
            StatusText = "Brak operacji do ponowienia";
            return;
        }

        history.Redo();
        StatusText = "Ponowiono operacje";
    }

    public string ExportSvg(string path)
    {
        var exporter = new SvgExporter();
        var svg = exporter.Export(CurrentDocument, new SvgExportOptions());
        File.WriteAllText(path, svg);
        StatusText = "Wyeksportowano SVG: " + path;
        return path;
    }

    public string ExportDxf(string path)
    {
        var exporter = new DxfExporter();
        var dxf = exporter.Export(CurrentDocument, new DxfExportOptions());
        File.WriteAllText(path, dxf);
        StatusText = "Wyeksportowano DXF: " + path;
        return path;
    }

    public CadDocument CreateKerfPreviewDocument(KerfCompensationOptions options, bool afterCompensation)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var document = new CadDocument(
            id: CurrentDocument.Id,
            name: afterCompensation ? CurrentDocument.Name + " - kerf" : CurrentDocument.Name,
            formatVersion: CurrentDocument.FormatVersion,
            parameters: CurrentDocument.Parameters,
            layers: CurrentDocument.Layers,
            materialProfile: CurrentDocument.MaterialProfile);

        foreach (var sketch in CurrentDocument.Sketches)
        {
            var preview = KerfCompensator.CreatePreview(sketch, options);
            document = document.AddSketch(afterCompensation ? preview.AfterCompensation : preview.BeforeCompensation);
        }

        StatusText = afterCompensation
            ? $"Podglad kerfu: {options.Mode}, {options.Kerf.Millimeters:0.###} mm"
            : "Podglad nominalny bez kompensacji kerfu";

        return document;
    }

    public void SetKerfCalibrationMeasurement(int slotIndex, double measuredWidthMillimeters)
    {
        if (slotIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(slotIndex), "Indeks szczeliny nie moze byc ujemny.");
        }

        if (measuredWidthMillimeters <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(measuredWidthMillimeters), "Pomiar musi byc wiekszy od zera.");
        }

        KerfCalibrationSlotIndex = slotIndex;
        KerfCalibrationMeasuredWidthMillimeters = measuredWidthMillimeters;
        KerfCalibrationRecommendedKerfMillimeters = KerfCalibrationCalculator.CalculateRecommendedKerf(
            new KerfCalibrationOptions(),
            slotIndex,
            Length.FromMillimeters(measuredWidthMillimeters)).Millimeters;
        StatusText = $"Rekomendowany kerf: {KerfCalibrationRecommendedKerfMillimeters:0.###} mm";
    }

    public void SaveKerfRecommendationToMaterialProfile()
    {
        if (KerfCalibrationRecommendedKerfMillimeters is null)
        {
            throw new InvalidOperationException("Najpierw zapisz pomiar kalibracji kerfu.");
        }

        var updatedProfile = SelectedMaterialProfile.WithDefaultKerf(
            Length.FromMillimeters(KerfCalibrationRecommendedKerfMillimeters.Value));
        var index = MaterialProfiles.IndexOf(SelectedMaterialProfile);
        if (index >= 0)
        {
            MaterialProfiles[index] = updatedProfile;
        }

        SelectedMaterialProfile = updatedProfile;
        ReplaceDocument(CurrentDocument.WithMaterialProfile(updatedProfile));
        StatusText = $"Zapisano kerf {updatedProfile.DefaultKerf.Millimeters:0.###} mm do profilu materialu";
    }

    private CadDocument CreateBoxDocument(BoxGeneratorOptions options, MaterialProfile materialProfile)
    {
        return new CadDocument(name: "Projekt pudelka")
            .WithMaterialProfile(materialProfile)
            .AddSketch(boxGenerator.GenerateSketch(options));
    }

    private void ReplaceDocument(CadDocument document)
    {
        history = new UndoRedoStack(document);
    }

    private void Execute(ICommand command)
    {
        history.Execute(command);
    }

    private void ExecuteCommands(IEnumerable<ICommand> commands, string successStatus)
    {
        var commandArray = commands.ToArray();
        if (commandArray.Length == 0)
        {
            StatusText = "Brak zaznaczonych encji";
            return;
        }

        Execute(commandArray.Length == 1 ? commandArray[0] : new CommandGroup(commandArray));

        StatusText = successStatus;
    }

    private Guid GetEditableSketchId()
    {
        return CurrentDocument.Sketches.FirstOrDefault()?.Id
            ?? throw new InvalidOperationException("Dokument nie ma szkicu do edycji.");
    }

    private IEnumerable<(Guid SketchId, Entity Entity)> FindEntities(IEnumerable<Guid> entityIds)
    {
        var ids = new HashSet<Guid>(entityIds);
        if (ids.Count == 0)
        {
            yield break;
        }

        foreach (var sketch in CurrentDocument.Sketches)
        {
            foreach (var entity in sketch.Entities)
            {
                if (ids.Contains(entity.Id))
                {
                    yield return (sketch.Id, entity);
                }
            }
        }
    }
}
