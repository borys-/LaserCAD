using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Generators;

/// <summary>
/// Generator prostych przegrodek z nacieciami montazowymi.
/// </summary>
public sealed class DividerGenerator : ISketchGenerator
{
    private const double DividerSpacingMillimeters = 8.0;
    private readonly RectangularGeneratorOptions options;
    private readonly int count;

    /// <summary>
    /// Tworzy generator przegrodek.
    /// </summary>
    public DividerGenerator(RectangularGeneratorOptions? options = null, int count = 3)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Divider generator must create at least one divider.");
        }

        this.options = options ?? new RectangularGeneratorOptions();
        this.count = count;
    }

    /// <inheritdoc />
    public string Name => "Divider generator";

    /// <inheritdoc />
    public Sketch GenerateSketch()
    {
        double width = options.Width.Millimeters;
        double depth = options.Depth.Millimeters;
        double slotWidth = options.MaterialThickness.Millimeters;
        double slotDepth = depth / 2.0;
        var entities = new List<Entity>();

        for (int index = 0; index < count; index++)
        {
            double offsetY = index * (depth + DividerSpacingMillimeters);
            entities.Add(new RectangleEntity(new Point2D(0.0, offsetY), width, depth, layerName: DefaultLayers.Cut.Name));

            double slotX = (width - slotWidth) / 2.0;
            entities.Add(new RectangleEntity(new Point2D(slotX, offsetY), slotWidth, slotDepth, layerName: DefaultLayers.Cut.Name));
        }

        return new Sketch(name: Name, entities: entities);
    }
}
