using LaserCad.Core.Documents;
using LaserCad.Core.MaterialModel;
using LaserCad.Core.Parameters;
using LaserCad.Core.Preview3D;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Documents;

public sealed class CadDocumentTests
{
    [Test]
    public void Constructor_ShouldCreateDocument()
    {
        var document = new CadDocument();

        Assert.That(document, Is.Not.Null);
    }

    [Test]
    public void Constructor_ShouldCreateDocumentId()
    {
        var document = new CadDocument();

        Assert.That(document.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Constructor_WithId_ShouldStoreId()
    {
        var id = Guid.NewGuid();

        var document = new CadDocument(id);

        Assert.That(document.Id, Is.EqualTo(id));
    }

    [Test]
    public void Constructor_WithEmptyId_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = new CadDocument(Guid.Empty));
    }

    [Test]
    public void Constructor_ShouldUseDefaultName()
    {
        var document = new CadDocument();

        Assert.That(document.Name, Is.EqualTo("Untitled"));
    }

    [Test]
    public void Constructor_WithName_ShouldStoreName()
    {
        var document = new CadDocument(name: "Box");

        Assert.That(document.Name, Is.EqualTo("Box"));
    }

    [Test]
    public void Constructor_WithEmptyName_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => _ = new CadDocument(name: ""));
    }

    [Test]
    public void Constructor_ShouldUseDefaultFormatVersion()
    {
        var document = new CadDocument();

        Assert.That(document.FormatVersion, Is.EqualTo(1));
    }

    [Test]
    public void Constructor_WithFormatVersion_ShouldStoreFormatVersion()
    {
        var document = new CadDocument(formatVersion: 2);

        Assert.That(document.FormatVersion, Is.EqualTo(2));
    }

    [Test]
    public void Constructor_WithInvalidFormatVersion_ShouldThrow()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _ = new CadDocument(formatVersion: 0));
    }

    [Test]
    public void Constructor_ShouldCreateEmptyParameterSet()
    {
        var document = new CadDocument();

        Assert.That(document.Parameters.Parameters, Is.Empty);
    }

    [Test]
    public void AddParameter_ShouldReturnDocumentWithAddedParameter()
    {
        var document = new CadDocument();
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 100.0);

        var updatedDocument = document.AddParameter(parameter);

        Assert.That(updatedDocument.Parameters.FindById(new ParameterId("Width")), Is.SameAs(parameter));
        Assert.That(document.Parameters.Parameters, Is.Empty);
    }

    [Test]
    public void Constructor_ShouldCreateDefaultLayerCollection()
    {
        var document = new CadDocument();

        Assert.That(document.Layers, Is.EqualTo(DefaultLayers.All));
    }

    [Test]
    public void AddLayer_ShouldReturnDocumentWithAddedLayer()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>());
        var layer = new Layer("Cut");

        var updatedDocument = document.AddLayer(layer);

        Assert.That(updatedDocument.Layers, Is.EqualTo(new[] { layer }));
        Assert.That(document.Layers, Is.Empty);
    }

    [Test]
    public void Constructor_ShouldCreateEmptySketchCollection()
    {
        var document = new CadDocument();

        Assert.That(document.Sketches, Is.Empty);
    }

    [Test]
    public void AddSketch_ShouldReturnDocumentWithAddedSketch()
    {
        var document = new CadDocument();
        var sketch = new Sketch();

        var updatedDocument = document.AddSketch(sketch);

        Assert.That(updatedDocument.Sketches, Is.EqualTo(new[] { sketch }));
        Assert.That(document.Sketches, Is.Empty);
    }

    [Test]
    public void Constructor_ShouldCreateEmptyGeneratorCollection()
    {
        var document = new CadDocument();

        Assert.That(document.Generators, Is.Empty);
    }

    [Test]
    public void AddGenerator_ShouldReturnDocumentWithAddedGenerator()
    {
        var document = new CadDocument();
        var generator = new GeneratorInstance();

        var updatedDocument = document.AddGenerator(generator);

        Assert.That(updatedDocument.Generators, Is.EqualTo(new[] { generator }));
        Assert.That(document.Generators, Is.Empty);
    }

    [Test]
    public void Constructor_ShouldUseNoMaterialProfileByDefault()
    {
        var document = new CadDocument();

        Assert.That(document.MaterialProfile, Is.Null);
    }

    [Test]
    public void WithMaterialProfile_ShouldReturnDocumentWithMaterialProfile()
    {
        var document = new CadDocument();
        var materialProfile = new MaterialProfile("Plywood 3 mm");

        var updatedDocument = document.WithMaterialProfile(materialProfile);

        Assert.That(updatedDocument.MaterialProfile, Is.SameAs(materialProfile));
        Assert.That(document.MaterialProfile, Is.Null);
    }

    [Test]
    public void Constructor_ShouldCreateEmptyDocument()
    {
        var document = new CadDocument();

        Assert.That(document.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(document.Name, Is.EqualTo("Untitled"));
        Assert.That(document.FormatVersion, Is.EqualTo(1));
        Assert.That(document.Parameters.Parameters, Is.Empty);
        Assert.That(document.Layers, Is.EqualTo(DefaultLayers.All));
        Assert.That(document.Sketches, Is.Empty);
        Assert.That(document.Generators, Is.Empty);
        Assert.That(document.MaterialProfile, Is.Null);
    }

    [Test]
    public void AddSketch_ShouldKeepDocumentMetadata()
    {
        var id = Guid.NewGuid();
        var document = new CadDocument(id, "Box", 2);
        var sketch = new Sketch(name: "Front");

        var updatedDocument = document.AddSketch(sketch);

        Assert.That(updatedDocument.Id, Is.EqualTo(id));
        Assert.That(updatedDocument.Name, Is.EqualTo("Box"));
        Assert.That(updatedDocument.FormatVersion, Is.EqualTo(2));
        Assert.That(updatedDocument.Sketches, Is.EqualTo(new[] { sketch }));
        Assert.That(document.Sketches, Is.Empty);
    }

    [Test]
    public void AddParameter_ShouldKeepDocumentMetadata()
    {
        var id = Guid.NewGuid();
        var document = new CadDocument(id, "Box", 2);
        var parameter = new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 100.0);

        var updatedDocument = document.AddParameter(parameter);

        Assert.That(updatedDocument.Id, Is.EqualTo(id));
        Assert.That(updatedDocument.Name, Is.EqualTo("Box"));
        Assert.That(updatedDocument.FormatVersion, Is.EqualTo(2));
        Assert.That(updatedDocument.Parameters.FindById(new ParameterId("Width")), Is.SameAs(parameter));
        Assert.That(document.Parameters.Parameters, Is.Empty);
    }

    [Test]
    public void MoveMaterialSolid_ShouldReturnDocumentWithMovedSolid()
    {
        var solid = CreateMaterialSolid();
        var document = new CadDocument().AddMaterialSolid(solid);

        var updatedDocument = document.MoveMaterialSolid(solid.Id, new Vector3D(1.0, 2.0, 3.0));

        Assert.That(updatedDocument.MaterialSolids.Single().Orientation.Position, Is.EqualTo(new Point3D(1.0, 2.0, 3.0)));
        Assert.That(document.MaterialSolids.Single().Orientation.Position, Is.EqualTo(new Point3D(0.0, 0.0, 0.0)));
    }

    [Test]
    public void RotateMaterialSolid_ShouldReturnDocumentWithRotatedSolid()
    {
        var solid = CreateMaterialSolid();
        var document = new CadDocument().AddMaterialSolid(solid);

        var updatedDocument = document.RotateMaterialSolid(solid.Id, Math.PI / 2.0);

        Assert.That(updatedDocument.MaterialSolids.Single().Orientation.RotationRadians, Is.EqualTo(Math.PI / 2.0));
        Assert.That(document.MaterialSolids.Single().Orientation.RotationRadians, Is.EqualTo(0.0));
    }

    [Test]
    public void RemoveMaterialSolid_ShouldReturnDocumentWithoutSolid()
    {
        var solid = CreateMaterialSolid();
        var document = new CadDocument().AddMaterialSolid(solid);

        var updatedDocument = document.RemoveMaterialSolid(solid.Id);

        Assert.That(updatedDocument.MaterialSolids, Is.Empty);
        Assert.That(document.MaterialSolids, Has.Count.EqualTo(1));
    }

    [Test]
    public void AddSlopedMaterialSolid_ShouldReturnDocumentWithSlopedSolid()
    {
        var solid = CreateSlopedMaterialSolid();
        var document = new CadDocument();

        var updatedDocument = document.AddSlopedMaterialSolid(solid);

        Assert.That(updatedDocument.SlopedMaterialSolids, Is.EqualTo(new[] { solid }));
        Assert.That(document.SlopedMaterialSolids, Is.Empty);
    }

    private static MaterialSolid CreateMaterialSolid()
    {
        return MaterialSolid.FromRectangle(
            "Plyta",
            new RectangleEntity(new Point2D(0.0, 0.0), 10.0, 5.0),
            DefaultMaterialProfiles.Plywood3Mm);
    }

    private static SlopedMaterialSolid CreateSlopedMaterialSolid()
    {
        return new SlopedMaterialSolid(
            "Bryla trapezowa",
            DefaultMaterialProfiles.Plywood3Mm,
            new SlopedMaterialSolidOptions(
                Length.FromMillimeters(120.0),
                Length.FromMillimeters(80.0),
                Length.FromMillimeters(50.0),
                Length.FromMillimeters(80.0)));
    }
}
