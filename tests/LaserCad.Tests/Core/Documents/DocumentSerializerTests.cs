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

    [Test]
    public void Deserialize_WithUnsupportedFormatVersion_ShouldThrow()
    {
        var id = Guid.NewGuid();
        var json = $$"""
        {
          "formatVersion": 999,
          "id": "{{id}}",
          "name": "Box"
        }
        """;
        var serializer = new DocumentSerializer();

        var exception = Assert.Throws<NotSupportedException>(() => _ = serializer.Deserialize(json));

        Assert.That(exception!.Message, Does.Contain("999"));
    }

    [Test]
    public void Serialize_WithUnsupportedFormatVersion_ShouldThrow()
    {
        var document = new CadDocument(formatVersion: 999);
        var serializer = new DocumentSerializer();

        var exception = Assert.Throws<NotSupportedException>(() => _ = serializer.Serialize(document));

        Assert.That(exception!.Message, Does.Contain("999"));
    }

    [Test]
    public void RoundTrip_WithEmptyDocument_ShouldPreserveDocument()
    {
        var id = Guid.NewGuid();
        var document = new CadDocument(id, "Box");
        var serializer = new DocumentSerializer();

        var json = serializer.Serialize(document);
        var roundTrippedDocument = serializer.Deserialize(json);

        Assert.That(roundTrippedDocument.Id, Is.EqualTo(id));
        Assert.That(roundTrippedDocument.Name, Is.EqualTo("Box"));
        Assert.That(roundTrippedDocument.FormatVersion, Is.EqualTo(DocumentSerializer.SupportedFormatVersion));
        Assert.That(roundTrippedDocument.Parameters.Parameters, Is.Empty);
        Assert.That(roundTrippedDocument.Layers.Select(layer => layer.Name), Is.EqualTo(document.Layers.Select(layer => layer.Name)));
        Assert.That(roundTrippedDocument.Layers.Select(layer => layer.Color), Is.EqualTo(document.Layers.Select(layer => layer.Color)));
        Assert.That(roundTrippedDocument.Layers.Select(layer => layer.Role), Is.EqualTo(document.Layers.Select(layer => layer.Role)));
        Assert.That(roundTrippedDocument.Sketches, Is.Empty);
        Assert.That(roundTrippedDocument.Generators, Is.Empty);
        Assert.That(roundTrippedDocument.MaterialProfile, Is.Null);
    }

    [Test]
    public void RoundTrip_WithParametersAndLayers_ShouldPreserveDocumentValues()
    {
        var id = Guid.NewGuid();
        var document = new CadDocument(
                id,
                "Parametric box",
                layers:
                [
                    new Layer("Cut", LayerColor.FromHex("#FF0000"), LayerRole.Cut),
                    new Layer("Labels", LayerColor.FromHex("#00AA00"), LayerRole.Engrave)
                ])
            .AddParameter(new Parameter(
                new ParameterId("Width"),
                "Width",
                ParameterType.Length,
                Length.FromMillimeters(120.0),
                "mm",
                Length.FromMillimeters(10.0),
                Length.FromMillimeters(300.0)))
            .AddParameter(new Parameter(new ParameterId("FingerCount"), "Finger count", ParameterType.Number, 7.0))
            .AddParameter(new Parameter(new ParameterId("HasLid"), "Has lid", ParameterType.Boolean, false))
            .AddParameter(new Parameter(new ParameterId("Label"), "Label", ParameterType.Text, "Front"))
            .AddParameter(new Parameter(new ParameterId("JointMode"), "Joint mode", ParameterType.Choice, "Tight"));
        var serializer = new DocumentSerializer();

        var json = serializer.Serialize(document);
        var roundTrippedDocument = serializer.Deserialize(json);
        var width = roundTrippedDocument.Parameters.FindById(new ParameterId("Width"));

        Assert.That(roundTrippedDocument.Id, Is.EqualTo(id));
        Assert.That(roundTrippedDocument.Name, Is.EqualTo("Parametric box"));
        Assert.That(roundTrippedDocument.Parameters.Parameters, Has.Count.EqualTo(5));
        Assert.That(width, Is.Not.Null);
        Assert.That(width!.Value, Is.EqualTo(Length.FromMillimeters(120.0)));
        Assert.That(width.DisplayUnit, Is.EqualTo("mm"));
        Assert.That(width.MinimumValue, Is.EqualTo(Length.FromMillimeters(10.0)));
        Assert.That(width.MaximumValue, Is.EqualTo(Length.FromMillimeters(300.0)));
        Assert.That(roundTrippedDocument.Parameters.FindById(new ParameterId("FingerCount"))!.Value, Is.EqualTo(7.0));
        Assert.That(roundTrippedDocument.Parameters.FindById(new ParameterId("HasLid"))!.Value, Is.False);
        Assert.That(roundTrippedDocument.Parameters.FindById(new ParameterId("Label"))!.Value, Is.EqualTo("Front"));
        Assert.That(roundTrippedDocument.Parameters.FindById(new ParameterId("JointMode"))!.Value, Is.EqualTo("Tight"));
        Assert.That(roundTrippedDocument.Layers, Has.Count.EqualTo(2));
        Assert.That(roundTrippedDocument.Layers[0].Name, Is.EqualTo("Cut"));
        Assert.That(roundTrippedDocument.Layers[0].Color, Is.EqualTo(LayerColor.FromHex("#FF0000")));
        Assert.That(roundTrippedDocument.Layers[0].Role, Is.EqualTo(LayerRole.Cut));
        Assert.That(roundTrippedDocument.Layers[1].Name, Is.EqualTo("Labels"));
        Assert.That(roundTrippedDocument.Layers[1].Color, Is.EqualTo(LayerColor.FromHex("#00AA00")));
        Assert.That(roundTrippedDocument.Layers[1].Role, Is.EqualTo(LayerRole.Engrave));
    }
}
