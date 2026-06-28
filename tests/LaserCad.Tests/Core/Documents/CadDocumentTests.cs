using LaserCad.Core.Documents;

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
}
