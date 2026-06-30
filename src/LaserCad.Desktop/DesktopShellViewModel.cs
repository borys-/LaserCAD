using System;
using System.Collections.ObjectModel;
using System.IO;
using LaserCad.Core.BoxGenerators;
using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
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
        Execute(new AddEntityCommand(GetEditableSketchId(), new RectangleEntity(new Point2D(0.0, 0.0), 40.0, 30.0, layerName: "Cut")));
        StatusText = "Dodano prostokat";
    }

    public void AddLine()
    {
        Execute(new AddEntityCommand(GetEditableSketchId(), new LineEntity(new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(60.0, 0.0)), layerName: "Cut")));
        StatusText = "Dodano linie";
    }

    public void AddCircle()
    {
        Execute(new AddEntityCommand(GetEditableSketchId(), new CircleEntity(new Circle2D(new Point2D(20.0, 20.0), 15.0), layerName: "Engrave")));
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
