using System.Text.Json;
using LaserCad.Core.Documents;
using LaserCad.Core.MaterialModel;
using LaserCad.Core.Parameters;
using LaserCad.Core.Preview3D;
using LaserCad.Geometry;
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
        Assert.That(document.MaterialSolids, Is.Empty);
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
        Assert.That(roundTrippedDocument.MaterialSolids, Is.Empty);
    }

    [Test]
    public void Serialize_WithMaterialSolid_ShouldWriteMaterialSolidValues()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;
        var rectangle = new RectangleEntity(new Point2D(0.0, 0.0), 20.0, 10.0);
        var solid = MaterialSolid.FromRectangle("Plyta frontowa", rectangle, material);
        var document = new CadDocument(layers: Array.Empty<Layer>()).AddMaterialSolid(solid);
        var serializer = new DocumentSerializer();

        var json = serializer.Serialize(document);
        using var parsedJson = JsonDocument.Parse(json);
        var materialSolid = parsedJson.RootElement.GetProperty("materialSolids")[0];

        Assert.That(materialSolid.GetProperty("id").GetGuid(), Is.EqualTo(solid.Id));
        Assert.That(materialSolid.GetProperty("name").GetString(), Is.EqualTo("Plyta frontowa"));
        Assert.That(materialSolid.GetProperty("materialProfile").GetProperty("thicknessMillimeters").GetDouble(), Is.EqualTo(3.0));
        Assert.That(materialSolid.GetProperty("mesh").GetProperty("vertices").GetArrayLength(), Is.EqualTo(8));
        Assert.That(materialSolid.GetProperty("mesh").GetProperty("triangleIndices").GetArrayLength(), Is.EqualTo(36));
        Assert.That(materialSolid.GetProperty("orientation").GetProperty("surfaceNormal").GetProperty("z").GetDouble(), Is.EqualTo(1.0));
    }

    [Test]
    public void RoundTrip_WithMaterialSolid_ShouldPreserveMaterialSolid()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;
        var rectangle = new RectangleEntity(new Point2D(0.0, 0.0), 20.0, 10.0);
        var orientation = new MaterialSolidOrientation(
            new Point3D(5.0, 6.0, 7.0),
            Math.PI / 4.0,
            new Vector3D(0.0, 1.0, 0.0));
        var mesh = MaterialSolid.FromRectangle("Plyta frontowa", rectangle, material).Mesh;
        var solid = new MaterialSolid(Guid.NewGuid(), "Plyta frontowa", material, mesh, orientation);
        var document = new CadDocument(layers: Array.Empty<Layer>()).AddMaterialSolid(solid);
        var serializer = new DocumentSerializer();

        var roundTrippedDocument = serializer.Deserialize(serializer.Serialize(document));
        var roundTrippedSolid = roundTrippedDocument.MaterialSolids.Single();

        Assert.That(roundTrippedSolid.Id, Is.EqualTo(solid.Id));
        Assert.That(roundTrippedSolid.Name, Is.EqualTo("Plyta frontowa"));
        Assert.That(roundTrippedSolid.MaterialProfile.Name, Is.EqualTo(material.Name));
        Assert.That(roundTrippedSolid.Thickness.Millimeters, Is.EqualTo(3.0));
        Assert.That(roundTrippedSolid.Mesh.Vertices, Is.EqualTo(solid.Mesh.Vertices));
        Assert.That(roundTrippedSolid.Mesh.TriangleIndices, Is.EqualTo(solid.Mesh.TriangleIndices));
        Assert.That(roundTrippedSolid.Orientation.Position, Is.EqualTo(new Point3D(5.0, 6.0, 7.0)));
        Assert.That(roundTrippedSolid.Orientation.RotationRadians, Is.EqualTo(Math.PI / 4.0));
        Assert.That(roundTrippedSolid.Orientation.SurfaceNormal, Is.EqualTo(new Vector3D(0.0, 1.0, 0.0)));
    }

    [Test]
    public void RoundTrip_WithMaterialSolidCutout_ShouldPreserveCutout()
    {
        var material = DefaultMaterialProfiles.Plywood3Mm;
        var rectangle = new RectangleEntity(new Point2D(0.0, 0.0), 100.0, 60.0);
        var cutout = CutoutFeature.Circle("Otwor", new Point2D(50.0, 30.0), 5.0);
        var solid = MaterialSolid.FromRectangle("Plyta frontowa", rectangle, material)
            .AddCutout(cutout, minimumBridgeMillimeters: 5.0);
        var document = new CadDocument(layers: Array.Empty<Layer>()).AddMaterialSolid(solid);
        var serializer = new DocumentSerializer();

        var roundTrippedDocument = serializer.Deserialize(serializer.Serialize(document));
        var roundTrippedCutout = roundTrippedDocument.MaterialSolids.Single().Cutouts.Single();

        Assert.That(roundTrippedCutout.Id, Is.EqualTo(cutout.Id));
        Assert.That(roundTrippedCutout.Name, Is.EqualTo("Otwor"));
        Assert.That(roundTrippedCutout.Kind, Is.EqualTo(CutoutFeatureKind.Circle));
        Assert.That(roundTrippedCutout.Contour.Vertices, Is.EqualTo(cutout.Contour.Vertices));
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

    [Test]
    public void RoundTrip_WithSketchAndEntities_ShouldPreserveSketchGeometry()
    {
        var sketchId = Guid.NewGuid();
        var lineId = Guid.NewGuid();
        var rectangleId = Guid.NewGuid();
        var circleId = Guid.NewGuid();
        var arcId = Guid.NewGuid();
        var polylineId = Guid.NewGuid();
        var textId = Guid.NewGuid();
        var document = new CadDocument(name: "Sketch document", layers: DefaultLayers.All)
            .AddSketch(new Sketch(
                sketchId,
                "Panel",
                [
                    new LineEntity(new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(10.0, 5.0)), lineId, "Cut"),
                    new RectangleEntity(new Point2D(1.0, 2.0), 30.0, 20.0, rectangleId, "Cut"),
                    new CircleEntity(new Circle2D(new Point2D(40.0, 20.0), 5.0), circleId, "Cut"),
                    new ArcEntity(new Arc2D(new Point2D(50.0, 25.0), 8.0, 0.25, 1.25, ArcDirection.Clockwise), arcId, "Score"),
                    new PolylineEntity(new Polyline2D(
                        [
                            new Point2D(0.0, 0.0),
                            new Point2D(5.0, 0.0),
                            new Point2D(5.0, 5.0)
                        ],
                        isClosed: true),
                        polylineId,
                        "Cut"),
                    new TextEntity("Front", new Point2D(3.0, 4.0), 6.0, textId, "Engrave")
                ]));
        var serializer = new DocumentSerializer();

        var json = serializer.Serialize(document);
        var roundTrippedDocument = serializer.Deserialize(json);
        var roundTrippedSketch = roundTrippedDocument.Sketches.Single();

        Assert.That(roundTrippedSketch.Id, Is.EqualTo(sketchId));
        Assert.That(roundTrippedSketch.Name, Is.EqualTo("Panel"));
        Assert.That(roundTrippedSketch.Entities, Has.Count.EqualTo(6));

        var line = (LineEntity)roundTrippedSketch.Entities[0];
        Assert.That(line.Id, Is.EqualTo(lineId));
        Assert.That(line.LayerName, Is.EqualTo("Cut"));
        Assert.That(line.Segment.Start, Is.EqualTo(new Point2D(0.0, 0.0)));
        Assert.That(line.Segment.End, Is.EqualTo(new Point2D(10.0, 5.0)));

        var rectangle = (RectangleEntity)roundTrippedSketch.Entities[1];
        Assert.That(rectangle.Id, Is.EqualTo(rectangleId));
        Assert.That(rectangle.Corners[0], Is.EqualTo(new Point2D(1.0, 2.0)));
        Assert.That(rectangle.Corners[2], Is.EqualTo(new Point2D(31.0, 22.0)));

        var circle = (CircleEntity)roundTrippedSketch.Entities[2];
        Assert.That(circle.Id, Is.EqualTo(circleId));
        Assert.That(circle.Circle.Center, Is.EqualTo(new Point2D(40.0, 20.0)));
        Assert.That(circle.Circle.Radius, Is.EqualTo(5.0));

        var arc = (ArcEntity)roundTrippedSketch.Entities[3];
        Assert.That(arc.Id, Is.EqualTo(arcId));
        Assert.That(arc.LayerName, Is.EqualTo("Score"));
        Assert.That(arc.Arc.Center, Is.EqualTo(new Point2D(50.0, 25.0)));
        Assert.That(arc.Arc.Radius, Is.EqualTo(8.0));
        Assert.That(arc.Arc.StartAngleRadians, Is.EqualTo(0.25));
        Assert.That(arc.Arc.EndAngleRadians, Is.EqualTo(1.25));
        Assert.That(arc.Arc.Direction, Is.EqualTo(ArcDirection.Clockwise));

        var polyline = (PolylineEntity)roundTrippedSketch.Entities[4];
        Assert.That(polyline.Id, Is.EqualTo(polylineId));
        Assert.That(polyline.Polyline.Points, Is.EqualTo(new[]
        {
            new Point2D(0.0, 0.0),
            new Point2D(5.0, 0.0),
            new Point2D(5.0, 5.0)
        }));
        Assert.That(polyline.Polyline.IsClosed, Is.True);

        var text = (TextEntity)roundTrippedSketch.Entities[5];
        Assert.That(text.Id, Is.EqualTo(textId));
        Assert.That(text.LayerName, Is.EqualTo("Engrave"));
        Assert.That(text.Text, Is.EqualTo("Front"));
        Assert.That(text.Font.FamilyName, Is.EqualTo("Arial"));
        Assert.That(text.Alignment, Is.EqualTo(TextAlignment.Left));
        Assert.That(text.Position, Is.EqualTo(new Point2D(3.0, 4.0)));
        Assert.That(text.Height, Is.EqualTo(6.0));
    }

    [Test]
    public void RoundTrip_WithTextFontAndAlignment_ShouldPreserveTextMetadata()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch().AddEntity(new TextEntity(
                "Label",
                new Point2D(2.0, 3.0),
                4.0,
                layerName: "Engrave",
                fontFamily: "Roboto",
                alignment: TextAlignment.Right,
                fontFilePath: "fonts/Roboto.ttf")));
        var serializer = new DocumentSerializer();

        var roundTripped = serializer.Deserialize(serializer.Serialize(document));
        var text = (TextEntity)roundTripped.Sketches[0].Entities[0];

        Assert.That(text.Font.FamilyName, Is.EqualTo("Roboto"));
        Assert.That(text.Font.FilePath, Is.EqualTo("fonts/Roboto.ttf"));
        Assert.That(text.Alignment, Is.EqualTo(TextAlignment.Right));
    }

    [Test]
    public void Serialize_WithSketchEntities_ShouldWriteEntityTypes()
    {
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch(entities:
            [
                new LineEntity(new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(10.0, 0.0))),
                new RectangleEntity(new Point2D(0.0, 0.0), 10.0, 5.0),
                new CircleEntity(new Circle2D(new Point2D(5.0, 5.0), 2.0)),
                new ArcEntity(new Arc2D(new Point2D(5.0, 5.0), 2.0, 0.0, 1.0)),
                new PolylineEntity(new Polyline2D([new Point2D(0.0, 0.0), new Point2D(1.0, 1.0)])),
                new TextEntity("Label", new Point2D(0.0, 0.0), 3.0)
            ]));
        var serializer = new DocumentSerializer();

        var json = serializer.Serialize(document);
        using var parsedJson = JsonDocument.Parse(json);
        var entities = parsedJson.RootElement.GetProperty("sketches")[0].GetProperty("entities");

        Assert.That(entities[0].GetProperty("type").GetString(), Is.EqualTo("Line"));
        Assert.That(entities[1].GetProperty("type").GetString(), Is.EqualTo("Rectangle"));
        Assert.That(entities[2].GetProperty("type").GetString(), Is.EqualTo("Circle"));
        Assert.That(entities[3].GetProperty("type").GetString(), Is.EqualTo("Arc"));
        Assert.That(entities[4].GetProperty("type").GetString(), Is.EqualTo("Polyline"));
        Assert.That(entities[5].GetProperty("type").GetString(), Is.EqualTo("Text"));
    }

    [Test]
    public void RoundTrip_WithDimensionBindings_ShouldPreserveBindings()
    {
        var widthId = new ParameterId("Width");
        var diameterId = new ParameterId("Diameter");
        var document = new CadDocument(layers: Array.Empty<Layer>())
            .AddSketch(new Sketch(entities:
            [
                new RectangleEntity(new Point2D(0.0, 0.0), 10.0, 5.0)
                    .BindDimension(new EntityDimensionBinding(EntityDimensionKind.Width, widthId)),
                new CircleEntity(new Circle2D(new Point2D(5.0, 5.0), 2.0))
                    .BindDimension(new EntityDimensionBinding(EntityDimensionKind.Diameter, diameterId))
            ]));
        var serializer = new DocumentSerializer();

        var roundTrippedDocument = serializer.Deserialize(serializer.Serialize(document));
        var rectangle = (RectangleEntity)roundTrippedDocument.Sketches.Single().Entities[0];
        var circle = (CircleEntity)roundTrippedDocument.Sketches.Single().Entities[1];

        Assert.That(rectangle.DimensionBindings, Has.Count.EqualTo(1));
        Assert.That(rectangle.DimensionBindings[0].Dimension, Is.EqualTo(EntityDimensionKind.Width));
        Assert.That(rectangle.DimensionBindings[0].ParameterId, Is.EqualTo(widthId));
        Assert.That(circle.DimensionBindings, Has.Count.EqualTo(1));
        Assert.That(circle.DimensionBindings[0].Dimension, Is.EqualTo(EntityDimensionKind.Diameter));
        Assert.That(circle.DimensionBindings[0].ParameterId, Is.EqualTo(diameterId));
    }
}
