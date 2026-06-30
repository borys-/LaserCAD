using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Generators;

/// <summary>
/// Generator prostego frontu szuflady z otworem uchwytu i linia pomocnicza dna.
/// </summary>
public sealed class DrawerGenerator : ISketchGenerator
{
    private readonly RectangularGeneratorOptions options;

    /// <summary>
    /// Tworzy generator szuflady.
    /// </summary>
    public DrawerGenerator(RectangularGeneratorOptions? options = null)
    {
        this.options = options ?? new RectangularGeneratorOptions();
    }

    /// <inheritdoc />
    public string Name => "Drawer generator";

    /// <inheritdoc />
    public Sketch GenerateSketch()
    {
        double width = options.Width.Millimeters;
        double depth = options.Depth.Millimeters;
        double handleRadius = Math.Max(options.MaterialThickness.Millimeters, Math.Min(width, depth) / 12.0);
        double scoreY = options.MaterialThickness.Millimeters;

        Entity[] entities =
        [
            new RectangleEntity(new Point2D(0.0, 0.0), width, depth, layerName: DefaultLayers.Cut.Name),
            new CircleEntity(new Circle2D(new Point2D(width / 2.0, depth / 2.0), handleRadius), layerName: DefaultLayers.Cut.Name),
            new LineEntity(new LineSegment2D(new Point2D(0.0, scoreY), new Point2D(width, scoreY)), layerName: DefaultLayers.Score.Name),
        ];

        return new Sketch(name: Name, entities: entities);
    }
}
