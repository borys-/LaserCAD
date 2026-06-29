using System.Text.Json;
using LaserCad.Core.Documents;

namespace LaserCad.Tests.Core.Documents;

public sealed class DocumentSerializerTests
{
    [Test]
    public void Serialize_WithEmptyDocument_ShouldWriteDocumentMetadata()
    {
        var id = Guid.NewGuid();
        var document = new CadDocument(id, "Box");
        var serializer = new DocumentSerializer();

        var json = serializer.Serialize(document);
        using var parsedJson = JsonDocument.Parse(json);

        Assert.That(parsedJson.RootElement.GetProperty("id").GetGuid(), Is.EqualTo(id));
        Assert.That(parsedJson.RootElement.GetProperty("name").GetString(), Is.EqualTo("Box"));
    }
}
