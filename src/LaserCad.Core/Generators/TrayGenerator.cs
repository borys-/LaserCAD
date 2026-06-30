using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Generators;

/// <summary>
/// Generator prostego rzutu tacki z obrysem ciecia i wewnetrznym dnem jako linia pomocnicza.
/// </summary>
public sealed class TrayGenerator : ISketchGenerator
{
    private readonly RectangularGeneratorOptions options;

    /// <summary>
    /// Tworzy generator tacki z podanymi albo domyslnymi opcjami.
    /// </summary>
    public TrayGenerator(RectangularGeneratorOptions? options = null)
    {
        this.options = options ?? new RectangularGeneratorOptions();
    }

    /// <inheritdoc />
    public string Name => "Tray generator";

    /// <inheritdoc />
    public Sketch GenerateSketch()
    {
        double width = options.Width.Millimeters;
        double depth = options.Depth.Millimeters;
        double inset = options.MaterialThickness.Millimeters;

        Entity[] entities =
        [
            new RectangleEntity(new Point2D(0.0, 0.0), width, depth, layerName: DefaultLayers.Cut.Name),
            new RectangleEntity(new Point2D(inset, inset), width - (2.0 * inset), depth - (2.0 * inset), layerName: DefaultLayers.Score.Name),
        ];

        return new Sketch(name: Name, entities: entities);
    }
}
