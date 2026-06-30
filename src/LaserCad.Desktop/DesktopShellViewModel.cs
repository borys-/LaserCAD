using System;
using System.Collections.ObjectModel;
using System.IO;
using LaserCad.Core.BoxGenerators;
using LaserCad.Core.Documents;
using LaserCad.Export.Dxf;
using LaserCad.Export.Svg;

namespace LaserCad.Desktop;

/// <summary>
/// Stan i akcje glownego desktop shell.
/// </summary>
public sealed class DesktopShellViewModel
{
    private readonly BoxGenerator boxGenerator = new();

    public DesktopShellViewModel()
    {
        MaterialProfiles = new ObservableCollection<MaterialProfile>(DefaultMaterialProfiles.All);
        SelectedMaterialProfile = DefaultMaterialProfiles.Plywood3Mm;
        BoxOptions = new BoxGeneratorOptions();
        CurrentDocument = CreateBoxDocument(BoxOptions, SelectedMaterialProfile);
        StatusText = "Gotowe";
    }

    public CadDocument CurrentDocument { get; private set; }

    public BoxGeneratorOptions BoxOptions { get; private set; }

    public ObservableCollection<MaterialProfile> MaterialProfiles { get; }

    public MaterialProfile SelectedMaterialProfile { get; private set; }

    public string StatusText { get; private set; }

    public void NewDocument()
    {
        CurrentDocument = new CadDocument(name: "Nowy projekt").WithMaterialProfile(SelectedMaterialProfile);
        StatusText = "Utworzono nowy projekt";
    }

    public void ApplyBoxOptions(BoxGeneratorOptions options)
    {
        BoxOptions = options ?? throw new ArgumentNullException(nameof(options));
        CurrentDocument = CreateBoxDocument(BoxOptions, SelectedMaterialProfile);
        StatusText = "Przebudowano podglad pudelka";
    }

    public void SetMaterialProfile(MaterialProfile materialProfile)
    {
        SelectedMaterialProfile = materialProfile ?? throw new ArgumentNullException(nameof(materialProfile));
        CurrentDocument = CurrentDocument.WithMaterialProfile(SelectedMaterialProfile);
        StatusText = "Zmieniono profil materialu";
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
}
