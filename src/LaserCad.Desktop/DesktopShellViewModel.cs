using System;
using System.Collections.ObjectModel;
using System.IO;
using LaserCad.Core.BoxGenerators;
using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Core.Generators;
using LaserCad.Core.Kerf;
using LaserCad.Core.Library;
using LaserCad.Core.MaterialModel;
using LaserCad.Core.Production;
using LaserCad.Export.Dxf;
using LaserCad.Export.Svg;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Desktop;

/// <summary>
/// Stan i akcje glownego desktop shell.
/// </summary>
public sealed class DesktopShellViewModel
{
    private readonly BoxGenerator boxGenerator = new();
    private readonly DocumentSerializer documentSerializer = new();
    private readonly MaterialUnfolder materialUnfolder = new();
    private readonly FlatPartNestingPlanner flatPartNestingPlanner = new();
    private readonly ProductionStatisticsCalculator productionStatisticsCalculator = new();
    private UndoRedoStack history;

    public DesktopShellViewModel()
    {
        var library = LoadProjectLibrary();
        LibraryTemplates = new ObservableCollection<LibraryTemplate>(library.Templates);
        MaterialProfiles = new ObservableCollection<MaterialProfile>(MergeProfiles(library.Materials));
        SelectedMaterialProfile = MaterialProfiles.FirstOrDefault(profile => profile.Name == DefaultMaterialProfiles.Plywood3Mm.Name)
            ?? DefaultMaterialProfiles.Plywood3Mm;
        SelectedLibraryTemplate = LibraryTemplates.FirstOrDefault();
        BoxOptions = new BoxGeneratorOptions();
        history = new UndoRedoStack(CreateBoxDocument(BoxOptions, SelectedMaterialProfile));
        StatusText = "Gotowe";
    }

    public CadDocument CurrentDocument => history.CurrentDocument;

    public BoxGeneratorOptions BoxOptions { get; private set; }

    public ObservableCollection<MaterialProfile> MaterialProfiles { get; }

    public ObservableCollection<LibraryTemplate> LibraryTemplates { get; }

    public MaterialProfile SelectedMaterialProfile { get; private set; }

    public LibraryTemplate? SelectedLibraryTemplate { get; private set; }

    public string StatusText { get; private set; }

    public string? CurrentProjectPath { get; private set; }

    public int UndoCount => history.UndoCount;

    public int RedoCount => history.RedoCount;

    public bool CanUndo => history.CanUndo;

    public bool CanRedo => history.CanRedo;

    public int KerfCalibrationSlotIndex { get; private set; }

    public double KerfCalibrationMeasuredWidthMillimeters { get; private set; }

    public double? KerfCalibrationRecommendedKerfMillimeters { get; private set; }

    public int LastNestedSheetCount { get; private set; }

    public int LastNestedPartCount { get; private set; }

    public double LastMaterialUsageRatio { get; private set; }

    public double LastCuttingLengthMillimeters { get; private set; }

    public void NewDocument()
    {
        ReplaceDocument(new CadDocument(name: "Nowy projekt").WithMaterialProfile(SelectedMaterialProfile));
        CurrentProjectPath = null;
        StatusText = "Utworzono nowy projekt";
    }

    public void SaveProject(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Sciezka projektu nie moze byc pusta.", nameof(path));
        }

        File.WriteAllText(path, documentSerializer.Serialize(CurrentDocument));
        CurrentProjectPath = path;
        StatusText = "Zapisano projekt: " + path;
    }

    public void LoadProject(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException("Sciezka projektu nie moze byc pusta.", nameof(path));
        }

        var document = documentSerializer.Deserialize(File.ReadAllText(path));
        ReplaceDocument(document);
        CurrentProjectPath = path;

        if (document.MaterialProfile is not null)
        {
            SelectedMaterialProfile = document.MaterialProfile;
            if (!MaterialProfiles.Any(profile => profile.Name == document.MaterialProfile.Name))
            {
                MaterialProfiles.Add(document.MaterialProfile);
            }
        }

        StatusText = "Wczytano projekt: " + path;
    }

    public void ApplyBoxOptions(BoxGeneratorOptions options)
    {
        BoxOptions = options ?? throw new ArgumentNullException(nameof(options));
        ReplaceDocument(CreateBoxDocument(BoxOptions, SelectedMaterialProfile));
        StatusText = "Przebudowano podglad pudelka";
    }

    public void SelectLibraryTemplate(LibraryTemplate? template)
    {
        SelectedLibraryTemplate = template;
    }

    public void ApplySelectedLibraryTemplate()
    {
        if (SelectedLibraryTemplate is null)
        {
            StatusText = "Brak wybranego szablonu";
            return;
        }

        ApplyLibraryTemplate(SelectedLibraryTemplate);
    }

    public void ApplyLibraryTemplate(LibraryTemplate template)
    {
        if (template is null)
        {
            throw new ArgumentNullException(nameof(template));
        }

        var document = new CadDocument(name: template.Name).WithMaterialProfile(SelectedMaterialProfile);
        var sketch = CreateSketchFromTemplate(template);
        ReplaceDocument(document.AddSketch(sketch));
        StatusText = "Wczytano szablon: " + template.Name;
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

    public void AddMaterialPlate(Point2D start, Point2D end)
    {
        var minX = Math.Min(start.X, end.X);
        var minY = Math.Min(start.Y, end.Y);
        var width = Math.Abs(end.X - start.X);
        var height = Math.Abs(end.Y - start.Y);

        if (width <= 0.0 || height <= 0.0)
        {
            StatusText = "Plyta materialowa wymaga dwoch roznych punktow";
            return;
        }

        var material = CurrentDocument.MaterialProfile ?? SelectedMaterialProfile;
        var rectangle = new RectangleEntity(new Point2D(minX, minY), width, height, layerName: "Cut");
        var solid = MaterialSolid.FromRectangle("Plyta materialowa", rectangle, material);
        ReplaceDocument(CurrentDocument.AddMaterialSolid(solid));
        StatusText = "Dodano plyte materialowa 3D";
    }

    public void AddDefaultSlopedMaterialSolid()
    {
        AddSlopedMaterialSolid(120.0, 80.0, 50.0, 80.0, SelectedMaterialProfile.DefaultKerf.Millimeters);
    }

    public void AddSlopedMaterialSolid(
        double widthMillimeters,
        double depthMillimeters,
        double frontHeightMillimeters,
        double backHeightMillimeters,
        double kerfMillimeters)
    {
        AddSlopedMaterialSolid(
            "Bryla z pochyla gora",
            widthMillimeters,
            depthMillimeters,
            frontHeightMillimeters,
            backHeightMillimeters,
            kerfMillimeters);
    }

    public void AddSlopedMaterialSolid(
        string constructionType,
        double widthMillimeters,
        double depthMillimeters,
        double frontHeightMillimeters,
        double backHeightMillimeters,
        double kerfMillimeters)
    {
        var material = SelectedMaterialProfile.WithDefaultKerf(Length.FromMillimeters(kerfMillimeters));
        var normalized = NormalizeSlopedConstruction(
            constructionType,
            material,
            frontHeightMillimeters,
            backHeightMillimeters);
        var solid = new SlopedMaterialSolid(
            normalized.Name,
            material,
            new SlopedMaterialSolidOptions(
                Length.FromMillimeters(widthMillimeters),
                Length.FromMillimeters(depthMillimeters),
                Length.FromMillimeters(normalized.FrontHeightMillimeters),
                Length.FromMillimeters(normalized.BackHeightMillimeters)));

        var document = CurrentDocument.SlopedMaterialSolids.Count == 0 && CurrentDocument.MaterialSolids.Count == 0
            ? new CadDocument(name: "Projekt bryly trapezowej").WithMaterialProfile(material)
            : CurrentDocument.WithMaterialProfile(material);

        ReplaceDocument(document.AddSlopedMaterialSolid(solid));
        StatusText = $"Dodano: {normalized.Name}, {widthMillimeters:0.#} x {depthMillimeters:0.#} mm";
    }

    public void AddCircularCutoutToLatestSlopedMaterialSolid(string faceName)
    {
        AddCircularCutoutToLatestSlopedMaterialSolid(faceName, 10.0, 20.0, 20.0);
    }

    public void AddCircularCutoutToLatestSlopedMaterialSolid(
        string faceName,
        double diameterMillimeters,
        double leftMillimeters,
        double bottomMillimeters)
    {
        if (string.IsNullOrWhiteSpace(faceName))
        {
            throw new ArgumentException("Nazwa sciany nie moze byc pusta.", nameof(faceName));
        }

        if (diameterMillimeters <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(diameterMillimeters), "Srednica otworu musi byc wieksza od zera.");
        }

        if (leftMillimeters < 0.0 || bottomMillimeters < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(leftMillimeters), "Odleglosci otworu od krawedzi nie moga byc ujemne.");
        }

        var solid = CurrentDocument.SlopedMaterialSolids.LastOrDefault();
        if (solid is null)
        {
            StatusText = "Najpierw dodaj bryle trapezowa";
            return;
        }

        var face = solid.Unfold()
            .FirstOrDefault(part => string.Equals(part.Name, faceName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException("Bryla nie ma sciany: " + faceName);
        var bounds = face.OuterContour.Bounds;
        var radiusMillimeters = diameterMillimeters / 2.0;
        var center = new Point2D(
            bounds.MinX + leftMillimeters,
            bounds.MinY + bottomMillimeters);
        var cutout = CutoutFeature.Circle("Otwor okragly", center, radiusMillimeters, face.Name);

        ReplaceDocument(CurrentDocument.AddCutoutToSlopedMaterialSolid(solid.Id, cutout));
        StatusText = $"Dodano otwor okragly: {face.Name}, fi {diameterMillimeters:0.#} mm";
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

    public IReadOnlyList<string> ExportNestedDxf(
        string directoryPath,
        double sheetWidthMillimeters,
        double sheetHeightMillimeters,
        double marginMillimeters,
        double spacingMillimeters,
        bool allowRotation)
    {
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            throw new ArgumentException("Katalog eksportu nie moze byc pusty.", nameof(directoryPath));
        }

        Directory.CreateDirectory(directoryPath);

        var sheet = new SheetSize(
            Length.FromMillimeters(sheetWidthMillimeters),
            Length.FromMillimeters(sheetHeightMillimeters),
            Length.FromMillimeters(marginMillimeters));
        var options = new NestingOptions(Length.FromMillimeters(spacingMillimeters), allowRotation);
        var flatParts = materialUnfolder.Unfold(
            CurrentDocument,
            new MaterialUnfoldingOptions(mergeIdenticalParts: true));
        var sheets = flatPartNestingPlanner.NestFlatPartsMultipleSheets(sheet, flatParts, options);
        var exporter = new NestedSheetDxfExporter();
        var export = exporter.Export(CurrentDocument.Name, sheet, sheets, new DxfExportOptions());
        var writtenFiles = new List<string>();

        foreach (var file in export.SheetFiles)
        {
            var path = Path.Combine(directoryPath, file.Key);
            File.WriteAllText(path, file.Value);
            writtenFiles.Add(path);
        }

        var combinedPath = Path.Combine(directoryPath, CurrentDocument.Name + "-all-sheets.dxf");
        File.WriteAllText(combinedPath, export.CombinedDxf);
        writtenFiles.Add(combinedPath);

        LastNestedSheetCount = sheets.Count;
        LastNestedPartCount = sheets.Sum(sheetResult => sheetResult.Parts.Count);
        StatusText = $"Wyeksportowano DXF nestingu: {writtenFiles.Count} plikow";
        return writtenFiles;
    }

    public CadDocument CreateNestingPreviewDocument(
        double sheetWidthMillimeters,
        double sheetHeightMillimeters,
        double marginMillimeters,
        double spacingMillimeters,
        bool allowRotation)
    {
        var sheet = new SheetSize(
            Length.FromMillimeters(sheetWidthMillimeters),
            Length.FromMillimeters(sheetHeightMillimeters),
            Length.FromMillimeters(marginMillimeters));
        var options = new NestingOptions(Length.FromMillimeters(spacingMillimeters), allowRotation);
        var flatParts = materialUnfolder.Unfold(
            CurrentDocument,
            new MaterialUnfoldingOptions(mergeIdenticalParts: true));

        if (flatParts.Count == 0)
        {
            StatusText = "Brak plyt 3D do przygotowania arkusza";
            LastNestedSheetCount = 0;
            LastNestedPartCount = 0;
            LastMaterialUsageRatio = 0.0;
            LastCuttingLengthMillimeters = 0.0;
            return CurrentDocument;
        }

        var sheets = flatPartNestingPlanner.NestMultipleSheets(sheet, flatParts, options);
        LastNestedSheetCount = sheets.Count;
        LastNestedPartCount = sheets.Sum(result => result.Parts.Count);
        var statistics = sheets
            .Select(result => productionStatisticsCalculator.Calculate(sheet, result))
            .ToArray();
        LastMaterialUsageRatio = statistics.Sum(item => item.MaterialUsageRatio) / Math.Max(1, sheets.Count);
        LastCuttingLengthMillimeters = statistics.Sum(item => item.CuttingLengthMillimeters);
        StatusText = $"Przygotowano nesting: {LastNestedPartCount} czesci / {LastNestedSheetCount} ark.";

        return CreateNestingPreviewDocument(sheet, sheets);
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

    private static ProjectLibrary LoadProjectLibrary()
    {
        var loader = new ProjectLibraryLoader();
        var libraryDirectory = Path.Combine(AppContext.BaseDirectory, "library");
        return loader.Load(libraryDirectory);
    }

    private static IEnumerable<MaterialProfile> MergeProfiles(IEnumerable<LibraryMaterialProfile> libraryMaterials)
    {
        var profiles = new List<MaterialProfile>();
        foreach (var material in libraryMaterials)
        {
            profiles.Add(material.Profile);
        }

        foreach (var profile in DefaultMaterialProfiles.All)
        {
            if (!profiles.Any(item => item.Name == profile.Name))
            {
                profiles.Add(profile);
            }
        }

        return profiles;
    }

    private static (string Name, double FrontHeightMillimeters, double BackHeightMillimeters) NormalizeSlopedConstruction(
        string constructionType,
        MaterialProfile material,
        double frontHeightMillimeters,
        double backHeightMillimeters)
    {
        var type = string.IsNullOrWhiteSpace(constructionType)
            ? "Bryla z pochyla gora"
            : constructionType;

        return type switch
        {
            "Prostopadloscian" => ("Prostopadloscian", frontHeightMillimeters, frontHeightMillimeters),
            "Klin" => ("Klin", Math.Max(material.Thickness.Millimeters, 0.1), Math.Max(frontHeightMillimeters, backHeightMillimeters)),
            "Obudowa z pochylonym panelem" => ("Obudowa z pochylonym panelem", frontHeightMillimeters, backHeightMillimeters),
            "Rynienka trapezowa" => ("Rynienka trapezowa", frontHeightMillimeters, backHeightMillimeters),
            _ => ("Bryla z pochyla gora", frontHeightMillimeters, backHeightMillimeters),
        };
    }

    private Sketch CreateSketchFromTemplate(LibraryTemplate template)
    {
        return template.GeneratorType switch
        {
            "Box" => CreateBoxSketchFromTemplate(template),
            "Organizer" => CreateOrganizerSketchFromTemplate(template),
            "Stand" => CreateStandSketchFromTemplate(template),
            _ => throw new InvalidOperationException("Nieobslugiwany typ generatora szablonu: " + template.GeneratorType),
        };
    }

    private Sketch CreateBoxSketchFromTemplate(LibraryTemplate template)
    {
        var options = new BoxGeneratorOptions(
            Length.FromMillimeters(ReadDouble(template, "widthMillimeters")),
            Length.FromMillimeters(ReadDouble(template, "depthMillimeters")),
            Length.FromMillimeters(ReadDouble(template, "heightMillimeters")),
            Length.FromMillimeters(ReadDouble(template, "materialThicknessMillimeters")),
            Length.FromMillimeters(ReadDouble(template, "kerfMillimeters")),
            Length.FromMillimeters(ReadDouble(template, "fingerWidthMillimeters")),
            Length.FromMillimeters(ReadDouble(template, "clearanceMillimeters")),
            ReadBoxType(template));
        BoxOptions = options;
        return boxGenerator.GenerateSketch(options);
    }

    private static Sketch CreateOrganizerSketchFromTemplate(LibraryTemplate template)
    {
        var options = CreateRectangularOptions(template);
        return new OrganizerGenerator(
            options,
            ReadInt(template, "columns"),
            ReadInt(template, "rows")).GenerateSketch();
    }

    private static Sketch CreateStandSketchFromTemplate(LibraryTemplate template)
    {
        return new StandGenerator(CreateRectangularOptions(template)).GenerateSketch();
    }

    private static RectangularGeneratorOptions CreateRectangularOptions(LibraryTemplate template)
    {
        return new RectangularGeneratorOptions(
            Length.FromMillimeters(ReadDouble(template, "widthMillimeters")),
            Length.FromMillimeters(ReadDouble(template, "depthMillimeters")),
            Length.FromMillimeters(ReadDouble(template, "materialThicknessMillimeters")));
    }

    private static BoxGeneratorType ReadBoxType(LibraryTemplate template)
    {
        var value = ReadString(template, "boxType");
        return Enum.TryParse<BoxGeneratorType>(value, ignoreCase: true, out var boxType)
            ? boxType
            : BoxGeneratorType.Open;
    }

    private static double ReadDouble(LibraryTemplate template, string key)
    {
        return Convert.ToDouble(ReadParameter(template, key));
    }

    private static int ReadInt(LibraryTemplate template, string key)
    {
        return Convert.ToInt32(ReadParameter(template, key));
    }

    private static string ReadString(LibraryTemplate template, string key)
    {
        return Convert.ToString(ReadParameter(template, key)) ?? string.Empty;
    }

    private static object ReadParameter(LibraryTemplate template, string key)
    {
        if (!template.Parameters.TryGetValue(key, out var value))
        {
            throw new InvalidOperationException("Szablon nie zawiera parametru: " + key);
        }

        return value;
    }

    private void ReplaceDocument(CadDocument document)
    {
        history = new UndoRedoStack(document);
    }

    private CadDocument CreateNestingPreviewDocument(SheetSize sheet, IReadOnlyList<NestingResult> sheets)
    {
        var document = new CadDocument(name: CurrentDocument.Name + " - nesting")
            .WithMaterialProfile(CurrentDocument.MaterialProfile ?? SelectedMaterialProfile);
        var sketch = new Sketch(name: "Nesting preview");
        var sheetGap = 20.0;

        for (var sheetIndex = 0; sheetIndex < sheets.Count; sheetIndex++)
        {
            var offsetX = sheetIndex * (sheet.Width.Millimeters + sheetGap);
            sketch = sketch.AddEntity(new RectangleEntity(
                new Point2D(offsetX, 0.0),
                sheet.Width.Millimeters,
                sheet.Height.Millimeters,
                layerName: "Ignore"));

            foreach (var part in sheets[sheetIndex].Parts)
            {
                sketch = sketch.AddEntity(new RectangleEntity(
                    new Point2D(offsetX + part.X.Millimeters, part.Y.Millimeters),
                    part.Width.Millimeters,
                    part.Height.Millimeters,
                    layerName: "Cut"));
            }
        }

        return document.AddSketch(sketch);
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
