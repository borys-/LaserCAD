using LaserCad.Core.Documents;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.Generators;

/// <summary>
/// Generator plyty pegboard z regularna siatka otworow.
/// </summary>
public sealed class PegboardGenerator : ISketchGenerator
{
    private readonly RectangularGeneratorOptions options;
    private readonly Length holeDiameter;
    private readonly Length holeSpacing;

    /// <summary>
    /// Tworzy generator pegboard.
    /// </summary>
    public PegboardGenerator(
        RectangularGeneratorOptions? options = null,
        Length? holeDiameter = null,
        Length? holeSpacing = null)
    {
        this.options = options ?? new RectangularGeneratorOptions();
        this.holeDiameter = EnsurePositive(holeDiameter ?? Length.FromMillimeters(5.0), nameof(holeDiameter));
        this.holeSpacing = EnsurePositive(holeSpacing ?? Length.FromMillimeters(20.0), nameof(holeSpacing));
    }

    /// <inheritdoc />
    public string Name => "Pegboard generator";

    /// <inheritdoc />
    public Sketch GenerateSketch()
    {
        double width = options.Width.Millimeters;
        double depth = options.Depth.Millimeters;
        double radius = holeDiameter.Millimeters / 2.0;
        double spacing = holeSpacing.Millimeters;
        var entities = new List<Entity>
        {
            new RectangleEntity(new Point2D(0.0, 0.0), width, depth, layerName: DefaultLayers.Cut.Name),
        };

        for (double y = spacing; y < depth; y += spacing)
        {
            for (double x = spacing; x < width; x += spacing)
            {
                entities.Add(new CircleEntity(new Circle2D(new Point2D(x, y), radius), layerName: DefaultLayers.Cut.Name));
            }
        }

        return new Sketch(name: Name, entities: entities);
    }

    private static Length EnsurePositive(Length value, string parameterName)
    {
        if (value <= Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(parameterName, "Pegboard hole dimension must be greater than zero.");
        }

        return value;
    }
}
