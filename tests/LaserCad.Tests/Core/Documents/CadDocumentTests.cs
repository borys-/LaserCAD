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
}
