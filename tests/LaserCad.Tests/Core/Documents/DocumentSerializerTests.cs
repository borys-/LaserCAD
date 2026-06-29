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

    [Test]
    public void Deserialize_WithEmptyDocumentJson_ShouldReadDocumentMetadata()
    {
        var id = Guid.NewGuid();
        var json = $$"""
        {
          "id": "{{id}}",
          "name": "Box"
        }
        """;
        var serializer = new DocumentSerializer();

        var document = serializer.Deserialize(json);

        Assert.That(document.Id, Is.EqualTo(id));
        Assert.That(document.Name, Is.EqualTo("Box"));
        Assert.That(document.Parameters.Parameters, Is.Empty);
        Assert.That(document.Layers, Is.EqualTo(DefaultLayers.All));
        Assert.That(document.Sketches, Is.Empty);
        Assert.That(document.Generators, Is.Empty);
        Assert.That(document.MaterialProfile, Is.Null);
    }
}
