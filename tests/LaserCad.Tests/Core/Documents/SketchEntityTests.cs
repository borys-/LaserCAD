using LaserCad.Core.Documents;
using LaserCad.Core.Parameters;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.Documents;

public sealed class SketchEntityTests
{
    [Test]
    public void Entity_ShouldImplementSketchEntityContract()
    {
        Assert.That(typeof(ISketchEntity).IsAssignableFrom(typeof(Entity)), Is.True);
    }

    [Test]
    public void LineEntity_ShouldStoreGeometryLayerAndBounds()
    {
        var entity = new LineEntity(
            new LineSegment2D(new Point2D(1.0, 2.0), new Point2D(4.0, 6.0)),
            layerName: "Score");

        Assert.That(entity.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(entity.LayerName, Is.EqualTo("Score"));
        Assert.That(entity.Bounds, Is.EqualTo(new BoundingBox(1.0, 2.0, 4.0, 6.0)));
    }

    [Test]
    public void RectangleEntity_ShouldCreateAxisAlignedRectangle()
    {
        var entity = new RectangleEntity(new Point2D(1.0, 2.0), 10.0, 5.0);

        Assert.That(entity.Corners, Has.Count.EqualTo(4));
        Assert.That(entity.Bounds, Is.EqualTo(new BoundingBox(1.0, 2.0, 11.0, 7.0)));
    }

    [Test]
    public void CircleEntity_ShouldStoreCircle()
    {
        var entity = new CircleEntity(new Circle2D(new Point2D(10.0, 20.0), 3.0));

        Assert.That(entity.Circle.Radius, Is.EqualTo(3.0));
        Assert.That(entity.Bounds, Is.EqualTo(new BoundingBox(7.0, 17.0, 13.0, 23.0)));
    }

    [Test]
    public void ArcEntity_ShouldStoreArc()
    {
        var entity = new ArcEntity(new Arc2D(new Point2D(0.0, 0.0), 10.0, 0.0, Math.PI / 2.0));

        Assert.That(entity.Arc.Radius, Is.EqualTo(10.0));
        Assert.That(entity.Bounds.MinX, Is.EqualTo(0.0).Within(GeometryTolerance.Default));
        Assert.That(entity.Bounds.MinY, Is.EqualTo(0.0).Within(GeometryTolerance.Default));
        Assert.That(entity.Bounds.MaxX, Is.EqualTo(10.0).Within(GeometryTolerance.Default));
        Assert.That(entity.Bounds.MaxY, Is.EqualTo(10.0).Within(GeometryTolerance.Default));
    }

    [Test]
    public void ArcEntity_Bounds_ShouldIncludeCardinalPointInsideSweep()
    {
        var entity = new ArcEntity(new Arc2D(new Point2D(0.0, 0.0), 10.0, Math.PI / 4.0, Math.PI * 3.0 / 4.0));

        Assert.That(entity.Bounds.MinX, Is.EqualTo(-Math.Sqrt(50.0)).Within(GeometryTolerance.Default));
        Assert.That(entity.Bounds.MaxY, Is.EqualTo(10.0).Within(GeometryTolerance.Default));
        Assert.That(entity.Bounds.MaxX, Is.EqualTo(Math.Sqrt(50.0)).Within(GeometryTolerance.Default));
    }

    [Test]
    public void PolylineEntity_ShouldStorePolyline()
    {
        var entity = new PolylineEntity(new Polyline2D(new[]
        {
            new Point2D(0.0, 0.0),
            new Point2D(4.0, 0.0),
            new Point2D(4.0, 3.0),
        }));

        Assert.That(entity.Polyline.Points, Has.Count.EqualTo(3));
        Assert.That(entity.Bounds, Is.EqualTo(new BoundingBox(0.0, 0.0, 4.0, 3.0)));
    }

    [Test]
    public void TextEntity_ShouldStoreTextPositionSizeFontAndAlignment()
    {
        var entity = new TextEntity(
            "Label",
            new Point2D(2.0, 3.0),
            5.0,
            layerName: "Engrave",
            fontFamily: "Roboto",
            alignment: TextAlignment.Center,
            fontFilePath: "fonts/Roboto.ttf");

        Assert.That(entity.Text, Is.EqualTo("Label"));
        Assert.That(entity.LayerName, Is.EqualTo("Engrave"));
        Assert.That(entity.Font.FamilyName, Is.EqualTo("Roboto"));
        Assert.That(entity.Font.FilePath, Is.EqualTo("fonts/Roboto.ttf"));
        Assert.That(entity.Alignment, Is.EqualTo(TextAlignment.Center));
        Assert.That(entity.Bounds, Is.EqualTo(new BoundingBox(-5.5, 3.0, 9.5, 8.0)));
    }

    [Test]
    public void TextEntity_ResolveParameters_ShouldReplaceParameterTokens()
    {
        var entity = new TextEntity("Panel {Width}", new Point2D(0.0, 0.0), 4.0);
        var parameters = new ParameterSet(new[]
        {
            new Parameter(new ParameterId("Width"), "Width", ParameterType.Number, 120.5),
        });

        var resolved = entity.ResolveParameters(parameters);

        Assert.That(resolved.Text, Is.EqualTo("Panel 120.5"));
        Assert.That(resolved.Id, Is.EqualTo(entity.Id));
    }

    [Test]
    public void TextToCurveConverter_ShouldCreateClosedCurveForEachVisibleCharacter()
    {
        var entity = new TextEntity("A B", new Point2D(10.0, 20.0), 5.0, layerName: "Engrave");
        var converter = new TextToCurveConverter();

        var curves = converter.ConvertToCurves(entity);

        Assert.That(curves, Has.Count.EqualTo(2));
        Assert.That(curves[0].LayerName, Is.EqualTo("Engrave"));
        Assert.That(curves[0].Polyline.IsClosed, Is.True);
        Assert.That(curves[0].Bounds, Is.EqualTo(new BoundingBox(10.0, 20.0, 13.0, 25.0)));
    }

    [Test]
    public void BindDimension_ShouldAssignEntityDimensionToParameter()
    {
        var binding = new EntityDimensionBinding(EntityDimensionKind.Width, new ParameterId("Width"));
        var rectangle = new RectangleEntity(new Point2D(0.0, 0.0), 10.0, 5.0);
        var circle = new CircleEntity(new Circle2D(new Point2D(0.0, 0.0), 5.0));

        var boundRectangle = rectangle.BindDimension(binding);
        var boundCircle = circle.BindDimension(new EntityDimensionBinding(EntityDimensionKind.Diameter, new ParameterId("Diameter")));

        Assert.That(boundRectangle.DimensionBindings, Has.Count.EqualTo(1));
        Assert.That(boundRectangle.DimensionBindings[0].Dimension, Is.EqualTo(EntityDimensionKind.Width));
        Assert.That(boundRectangle.DimensionBindings[0].ParameterId, Is.EqualTo(new ParameterId("Width")));
        Assert.That(boundCircle.DimensionBindings[0].Dimension, Is.EqualTo(EntityDimensionKind.Diameter));
    }

    [Test]
    public void RectangleEntity_RebuildFromParameters_ShouldUpdateWidth()
    {
        var rectangle = new RectangleEntity(new Point2D(2.0, 3.0), 10.0, 5.0)
            .BindDimension(new EntityDimensionBinding(EntityDimensionKind.Width, new ParameterId("Width")));
        var parameters = new ParameterSet(new[]
        {
            new Parameter(new ParameterId("Width"), "Width", ParameterType.Length, Length.FromMillimeters(25.0)),
        });

        var rebuilt = rectangle.RebuildFromParameters(parameters);

        Assert.That(rebuilt.Bounds, Is.EqualTo(new BoundingBox(2.0, 3.0, 27.0, 8.0)));
        Assert.That(rebuilt.Id, Is.EqualTo(rectangle.Id));
        Assert.That(rebuilt.DimensionBindings, Has.Count.EqualTo(1));
    }

    [Test]
    public void RectangleEntity_RebuildFromParameters_ShouldUpdateHeight()
    {
        var rectangle = new RectangleEntity(new Point2D(2.0, 3.0), 10.0, 5.0)
            .BindDimension(new EntityDimensionBinding(EntityDimensionKind.Height, new ParameterId("Height")));
        var parameters = new ParameterSet(new[]
        {
            new Parameter(new ParameterId("Height"), "Height", ParameterType.Length, Length.FromMillimeters(15.0)),
        });

        var rebuilt = rectangle.RebuildFromParameters(parameters);

        Assert.That(rebuilt.Bounds, Is.EqualTo(new BoundingBox(2.0, 3.0, 12.0, 18.0)));
        Assert.That(rebuilt.Id, Is.EqualTo(rectangle.Id));
        Assert.That(rebuilt.DimensionBindings, Has.Count.EqualTo(1));
    }

    [Test]
    public void CircleEntity_RebuildFromParameters_ShouldUpdateDiameter()
    {
        var circle = new CircleEntity(new Circle2D(new Point2D(10.0, 20.0), 3.0))
            .BindDimension(new EntityDimensionBinding(EntityDimensionKind.Diameter, new ParameterId("Diameter")));
        var parameters = new ParameterSet(new[]
        {
            new Parameter(new ParameterId("Diameter"), "Diameter", ParameterType.Length, Length.FromMillimeters(20.0)),
        });

        var rebuilt = circle.RebuildFromParameters(parameters);

        Assert.That(rebuilt.Circle.Center, Is.EqualTo(new Point2D(10.0, 20.0)));
        Assert.That(rebuilt.Circle.Radius, Is.EqualTo(10.0));
        Assert.That(rebuilt.Id, Is.EqualTo(circle.Id));
        Assert.That(rebuilt.DimensionBindings, Has.Count.EqualTo(1));
    }

    [Test]
    public void Transform_ShouldKeepIdentityAndLayer()
    {
        var id = Guid.NewGuid();
        var entity = new LineEntity(
            new LineSegment2D(new Point2D(1.0, 2.0), new Point2D(3.0, 4.0)),
            id,
            "Cut");

        var transformed = (LineEntity)entity.Transform(Matrix3x3.CreateTranslation(10.0, 20.0));

        Assert.That(transformed.Id, Is.EqualTo(id));
        Assert.That(transformed.LayerName, Is.EqualTo("Cut"));
        Assert.That(transformed.Segment.Start, Is.EqualTo(new Point2D(11.0, 22.0)));
        Assert.That(transformed.Segment.End, Is.EqualTo(new Point2D(13.0, 24.0)));
    }
}
