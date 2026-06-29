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
        var document = new CadDocument(id, "Box", formatVersion: 1);
        var serializer = new DocumentSerializer();

        var json = serializer.Serialize(document);
        using var parsedJson = JsonDocument.Parse(json);

        Assert.That(parsedJson.RootElement.GetProperty("formatVersion").GetInt32(), Is.EqualTo(1));
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
        Assert.That(document.FormatVersion, Is.EqualTo(DocumentSerializer.SupportedFormatVersion));
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

    [Test]
    public void Serialize_WithLayers_ShouldWriteLayerValues()
    {
        var document = new CadDocument(layers:
        [
            new Layer("Cut", LayerColor.FromHex("#ff0000"), LayerRole.Cut),
            new Layer("Engrave", LayerColor.FromHex("#0000ff"), LayerRole.Engrave)
        ]);
        var serializer = new DocumentSerializer();

        var json = serializer.Serialize(document);
        using var parsedJson = JsonDocument.Parse(json);
        var layers = parsedJson.RootElement.GetProperty("layers");

        Assert.That(layers.GetArrayLength(), Is.EqualTo(2));
        Assert.That(layers[0].GetProperty("name").GetString(), Is.EqualTo("Cut"));
        Assert.That(layers[0].GetProperty("color").GetString(), Is.EqualTo("#FF0000"));
        Assert.That(layers[0].GetProperty("role").GetString(), Is.EqualTo("Cut"));
        Assert.That(layers[1].GetProperty("role").GetString(), Is.EqualTo("Engrave"));
    }

    [Test]
    public void Deserialize_WithLayers_ShouldReadLayerValues()
    {
        var id = Guid.NewGuid();
        var json = $$"""
        {
          "id": "{{id}}",
          "name": "Box",
          "layers": [
            {
              "name": "Cut",
              "color": "#FF0000",
              "role": "Cut"
            },
            {
              "name": "Engrave",
              "color": "#0000FF",
              "role": "Engrave"
            }
          ]
        }
        """;
        var serializer = new DocumentSerializer();

        var document = serializer.Deserialize(json);

        Assert.That(document.Layers, Has.Count.EqualTo(2));
        Assert.That(document.Layers[0].Name, Is.EqualTo("Cut"));
        Assert.That(document.Layers[0].Color, Is.EqualTo(LayerColor.FromHex("#FF0000")));
        Assert.That(document.Layers[0].Role, Is.EqualTo(LayerRole.Cut));
        Assert.That(document.Layers[1].Role, Is.EqualTo(LayerRole.Engrave));
    }

    [Test]
    public void Deserialize_WithEmptyLayers_ShouldKeepEmptyLayerCollection()
    {
        var id = Guid.NewGuid();
        var json = $$"""
        {
          "id": "{{id}}",
          "name": "Box",
          "layers": []
        }
        """;
        var serializer = new DocumentSerializer();

        var document = serializer.Deserialize(json);

        Assert.That(document.Layers, Is.Empty);
    }

    [Test]
    public void Serialize_WithMaterialProfile_ShouldWriteMaterialProfileValues()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .WithMaterialProfile(new MaterialProfile(
                "Plywood 3 mm",
                Length.FromMillimeters(3.0),
                Length.FromMillimeters(0.15),
                Length.FromMillimeters(0.1),
                Length.FromMillimeters(3.0)));
        var serializer = new DocumentSerializer();

        var json = serializer.Serialize(document);
        using var parsedJson = JsonDocument.Parse(json);
        var materialProfile = parsedJson.RootElement.GetProperty("materialProfile");

        Assert.That(materialProfile.GetProperty("name").GetString(), Is.EqualTo("Plywood 3 mm"));
        Assert.That(materialProfile.GetProperty("thicknessMillimeters").GetDouble(), Is.EqualTo(3.0));
        Assert.That(materialProfile.GetProperty("defaultKerfMillimeters").GetDouble(), Is.EqualTo(0.15));
        Assert.That(materialProfile.GetProperty("defaultClearanceMillimeters").GetDouble(), Is.EqualTo(0.1));
        Assert.That(materialProfile.GetProperty("minimumFingerWidthMillimeters").GetDouble(), Is.EqualTo(3.0));
    }

    [Test]
    public void Deserialize_WithMaterialProfile_ShouldReadMaterialProfileValues()
    {
        var id = Guid.NewGuid();
        var json = $$"""
        {
          "id": "{{id}}",
          "name": "Box",
          "materialProfile": {
            "name": "Plywood 3 mm",
            "thicknessMillimeters": 3,
            "defaultKerfMillimeters": 0.15,
            "defaultClearanceMillimeters": 0.1,
            "minimumFingerWidthMillimeters": 3
          }
        }
        """;
        var serializer = new DocumentSerializer();

        var document = serializer.Deserialize(json);

        Assert.That(document.MaterialProfile, Is.Not.Null);
        Assert.That(document.MaterialProfile!.Name, Is.EqualTo("Plywood 3 mm"));
        Assert.That(document.MaterialProfile.Thickness, Is.EqualTo(Length.FromMillimeters(3.0)));
        Assert.That(document.MaterialProfile.DefaultKerf, Is.EqualTo(Length.FromMillimeters(0.15)));
        Assert.That(document.MaterialProfile.DefaultClearance, Is.EqualTo(Length.FromMillimeters(0.1)));
        Assert.That(document.MaterialProfile.MinimumFingerWidth, Is.EqualTo(Length.FromMillimeters(3.0)));
    }

    [Test]
    public void Deserialize_WithFormatVersion_ShouldReadFormatVersion()
    {
        var id = Guid.NewGuid();
        var json = $$"""
        {
          "formatVersion": 1,
          "id": "{{id}}",
          "name": "Box"
        }
        """;
        var serializer = new DocumentSerializer();

        var document = serializer.Deserialize(json);

        Assert.That(document.FormatVersion, Is.EqualTo(1));
    }
}
