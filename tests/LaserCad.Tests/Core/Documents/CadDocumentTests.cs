using LaserCad.Core.Documents;
using LaserCad.Core.Parameters;

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
}
