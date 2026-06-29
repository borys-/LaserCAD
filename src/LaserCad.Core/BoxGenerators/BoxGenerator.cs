using LaserCad.Core.Documents;
using LaserCad.Core.FingerJoints;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.BoxGenerators;

/// <summary>
/// Generator geometrii 2D dla parametrycznego pudelka z polaczeniami palcowymi.
/// </summary>
public sealed class BoxGenerator
{
    private const double DefaultPanelSpacingMillimeters = 10.0;
    private readonly FingerJointGenerator fingerJointGenerator;

    /// <summary>
    /// Tworzy generator pudelka z domyslnym generatorem profili finger joint.
    /// </summary>
    public BoxGenerator()
        : this(new FingerJointGenerator())
    {
    }

    /// <summary>
    /// Tworzy generator pudelka z jawnie podanym generatorem profili finger joint.
    /// </summary>
    public BoxGenerator(FingerJointGenerator fingerJointGenerator)
    {
        this.fingerJointGenerator = fingerJointGenerator ?? throw new ArgumentNullException(nameof(fingerJointGenerator));
    }

    /// <summary>
    /// Generuje szkic z rozlozonymi na plaszczyznie sciankami pudelka.
    /// </summary>
    public Sketch GenerateSketch(BoxGeneratorOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var panels = CreatePanels(options);
        double cursorX = 0.0;
        var entities = new List<Entity>(panels.Count);

        foreach (BoxPanel panel in panels)
        {
            entities.Add(CreatePanelEntity(panel, cursorX, 0.0, options));
            cursorX += panel.Width.Millimeters + DefaultPanelSpacingMillimeters;
        }

        return new Sketch(name: "Box generator", entities: entities);
    }

    private static IReadOnlyList<BoxPanel> CreatePanels(BoxGeneratorOptions options)
    {
        var panels = new List<BoxPanel>
        {
            new("Front", options.Width, options.Height),
            new("Back", options.Width, options.Height),
            new("Left", options.Depth, options.Height),
            new("Right", options.Depth, options.Height),
            new("Bottom", options.Width, options.Depth),
        };

        if (options.BoxType == BoxGeneratorType.WithLid)
        {
            panels.Add(new BoxPanel("Lid", options.Width, options.Depth));
        }

        return panels;
    }

    private PolylineEntity CreatePanelEntity(BoxPanel panel, double originX, double originY, BoxGeneratorOptions options)
    {
        IReadOnlyList<Point2D> points = CreateFingerJointRectangle(originX, originY, panel.Width, panel.Height, options);

        return new PolylineEntity(new Polyline2D(points, isClosed: true), layerName: DefaultLayers.Cut.Name);
    }

    private IReadOnlyList<Point2D> CreateFingerJointRectangle(
        double originX,
        double originY,
        Length width,
        Length height,
        BoxGeneratorOptions options)
    {
        Point2D bottomLeft = new(originX, originY);
        Point2D topLeft = new(originX, originY + height.Millimeters);
        Point2D topRight = new(originX + width.Millimeters, originY + height.Millimeters);
        Point2D bottomRight = new(originX + width.Millimeters, originY);
        var points = new List<Point2D>();

        AppendEdge(points, new LineSegment2D(bottomLeft, topLeft), options);
        AppendEdge(points, new LineSegment2D(topLeft, topRight), options);
        AppendEdge(points, new LineSegment2D(topRight, bottomRight), options);
        AppendEdge(points, new LineSegment2D(bottomRight, bottomLeft), options);

        return points;
    }

    private void AppendEdge(List<Point2D> points, LineSegment2D edge, BoxGeneratorOptions options)
    {
        FingerJointProfile profile = fingerJointGenerator.GenerateEdge(
            edge,
            options.MaterialThickness,
            CreateFingerJointOptions(options));

        foreach (Point2D point in profile.Points)
        {
            if (points.Count > 0 && points[^1].DistanceTo(point) <= GeometryTolerance.Default)
            {
                continue;
            }

            points.Add(point);
        }
    }

    private static FingerJointOptions CreateFingerJointOptions(BoxGeneratorOptions options)
    {
        Length tolerance = options.FingerWidth / 2.0;

        return new FingerJointOptions(
            fingerWidth: options.FingerWidth,
            minimumFingerWidth: options.FingerWidth - tolerance,
            maximumFingerWidth: options.FingerWidth + tolerance,
            kerf: options.Kerf,
            clearance: options.Clearance);
    }

    private sealed record BoxPanel(string Name, Length Width, Length Height);
}
