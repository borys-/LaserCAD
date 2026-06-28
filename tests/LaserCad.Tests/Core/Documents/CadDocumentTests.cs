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
    public void Constructor_ShouldCreateEmptyLayerCollection()
    {
        var document = new CadDocument();

        Assert.That(document.Layers, Is.Empty);
    }

    [Test]
    public void AddLayer_ShouldReturnDocumentWithAddedLayer()
    {
        var document = new CadDocument();
        var layer = new Layer("Cut");

        var updatedDocument = document.AddLayer(layer);

        Assert.That(updatedDocument.Layers, Is.EqualTo(new[] { layer }));
        Assert.That(document.Layers, Is.Empty);
    }
}
