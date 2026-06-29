using System.Text.Json;
using LaserCad.Core.Documents;
using LaserCad.Core.Parameters;
using LaserCad.Geometry.Units;

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

    [Test]
    public void Serialize_WithParameters_ShouldWriteParameterValues()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddParameter(new Parameter(
                new ParameterId("Width"),
                "Width",
                ParameterType.Length,
                Length.FromMillimeters(120.0),
                "mm",
                Length.FromMillimeters(10.0),
                Length.FromMillimeters(300.0)))
            .AddParameter(new Parameter(new ParameterId("HasLid"), "Has lid", ParameterType.Boolean, true));
        var serializer = new DocumentSerializer();

        var json = serializer.Serialize(document);
        using var parsedJson = JsonDocument.Parse(json);
        var parameters = parsedJson.RootElement.GetProperty("parameters");

        Assert.That(parameters.GetArrayLength(), Is.EqualTo(2));
        Assert.That(parameters[0].GetProperty("id").GetString(), Is.EqualTo("Width"));
        Assert.That(parameters[0].GetProperty("type").GetString(), Is.EqualTo("Length"));
        Assert.That(parameters[0].GetProperty("value").GetDouble(), Is.EqualTo(120.0));
        Assert.That(parameters[0].GetProperty("minimumValue").GetDouble(), Is.EqualTo(10.0));
        Assert.That(parameters[0].GetProperty("maximumValue").GetDouble(), Is.EqualTo(300.0));
        Assert.That(parameters[1].GetProperty("value").GetBoolean(), Is.True);
    }

    [Test]
    public void Deserialize_WithParameters_ShouldReadParameterValues()
    {
        var id = Guid.NewGuid();
        var json = $$"""
        {
          "id": "{{id}}",
          "name": "Box",
          "parameters": [
            {
              "id": "Width",
              "name": "Width",
              "type": "Length",
              "value": 120,
              "displayUnit": "mm",
              "minimumValue": 10,
              "maximumValue": 300
            },
            {
              "id": "HasLid",
              "name": "Has lid",
              "type": "Boolean",
              "value": true
            }
          ]
        }
        """;
        var serializer = new DocumentSerializer();

        var document = serializer.Deserialize(json);
        var width = document.Parameters.FindById(new ParameterId("Width"));
        var hasLid = document.Parameters.FindById(new ParameterId("HasLid"));

        Assert.That(width, Is.Not.Null);
        Assert.That(width!.Value, Is.EqualTo(Length.FromMillimeters(120.0)));
        Assert.That(width.MinimumValue, Is.EqualTo(Length.FromMillimeters(10.0)));
        Assert.That(width.MaximumValue, Is.EqualTo(Length.FromMillimeters(300.0)));
        Assert.That(hasLid, Is.Not.Null);
        Assert.That(hasLid!.Value, Is.True);
    }
}
