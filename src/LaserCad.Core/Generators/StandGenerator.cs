using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Generators;

/// <summary>
/// Generator prostej podstawki z dwiema plytami krzyzowymi i nacieciami.
/// </summary>
public sealed class StandGenerator : ISketchGenerator
{
    private const double PartSpacingMillimeters = 8.0;
    private readonly RectangularGeneratorOptions options;

    /// <summary>
    /// Tworzy generator podstawki.
    /// </summary>
    public StandGenerator(RectangularGeneratorOptions? options = null)
    {
        this.options = options ?? new RectangularGeneratorOptions();
    }

    /// <inheritdoc />
    public string Name => "Stand generator";

    /// <inheritdoc />
    public Sketch GenerateSketch()
    {
        double width = options.Width.Millimeters;
        double depth = options.Depth.Millimeters;
        double slotWidth = options.MaterialThickness.Millimeters;
        double slotDepth = depth / 2.0;
        double secondPartY = depth + PartSpacingMillimeters;
        double slotX = (width - slotWidth) / 2.0;

        Entity[] entities =
        [
            new RectangleEntity(new Point2D(0.0, 0.0), width, depth, layerName: DefaultLayers.Cut.Name),
            new RectangleEntity(new Point2D(slotX, 0.0), slotWidth, slotDepth, layerName: DefaultLayers.Cut.Name),
            new RectangleEntity(new Point2D(0.0, secondPartY), width, depth, layerName: DefaultLayers.Cut.Name),
            new RectangleEntity(new Point2D(slotX, secondPartY + slotDepth), slotWidth, slotDepth, layerName: DefaultLayers.Cut.Name),
        ];

        return new Sketch(name: Name, entities: entities);
    }
}
