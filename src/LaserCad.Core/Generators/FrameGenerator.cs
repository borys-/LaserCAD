using LaserCad.Core.Documents;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.Generators;

/// <summary>
/// Generator prostej ramki z obrysem zewnetrznym i wewnetrznym wycieciem.
/// </summary>
public sealed class FrameGenerator : ISketchGenerator
{
    private readonly RectangularGeneratorOptions options;
    private readonly Length borderWidth;

    /// <summary>
    /// Tworzy generator ramki.
    /// </summary>
    public FrameGenerator(RectangularGeneratorOptions? options = null, Length? borderWidth = null)
    {
        this.options = options ?? new RectangularGeneratorOptions();
        this.borderWidth = EnsurePositive(borderWidth ?? Length.FromMillimeters(12.0), nameof(borderWidth));
        EnsureBorderFits();
    }

    /// <inheritdoc />
    public string Name => "Frame generator";

    /// <inheritdoc />
    public Sketch GenerateSketch()
    {
        double width = options.Width.Millimeters;
        double depth = options.Depth.Millimeters;
        double border = borderWidth.Millimeters;

        Entity[] entities =
        [
            new RectangleEntity(new Point2D(0.0, 0.0), width, depth, layerName: DefaultLayers.Cut.Name),
            new RectangleEntity(new Point2D(border, border), width - (2.0 * border), depth - (2.0 * border), layerName: DefaultLayers.Cut.Name),
        ];

        return new Sketch(name: Name, entities: entities);
    }

    private void EnsureBorderFits()
    {
        if (borderWidth.Millimeters * 2.0 >= Math.Min(options.Width.Millimeters, options.Depth.Millimeters))
        {
            throw new ArgumentOutOfRangeException(nameof(borderWidth), "Frame border must leave a positive inner opening.");
        }
    }

    private static Length EnsurePositive(Length value, string parameterName)
    {
        if (value <= Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(parameterName, "Frame border must be greater than zero.");
        }

        return value;
    }
}
