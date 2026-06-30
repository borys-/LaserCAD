using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Generators;

/// <summary>
/// Generator prostego organizera z prostokatnym obrysem i siatka przegrod.
/// </summary>
public sealed class OrganizerGenerator : ISketchGenerator
{
    private readonly RectangularGeneratorOptions options;
    private readonly int columns;
    private readonly int rows;

    /// <summary>
    /// Tworzy generator organizera.
    /// </summary>
    public OrganizerGenerator(RectangularGeneratorOptions? options = null, int columns = 3, int rows = 2)
    {
        if (columns < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(columns), "Organizer must have at least one column.");
        }

        if (rows < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(rows), "Organizer must have at least one row.");
        }

        this.options = options ?? new RectangularGeneratorOptions();
        this.columns = columns;
        this.rows = rows;
    }

    /// <inheritdoc />
    public string Name => "Organizer generator";

    /// <inheritdoc />
    public Sketch GenerateSketch()
    {
        double width = options.Width.Millimeters;
        double depth = options.Depth.Millimeters;
        var entities = new List<Entity>
        {
            new RectangleEntity(new Point2D(0.0, 0.0), width, depth, layerName: DefaultLayers.Cut.Name),
        };

        for (int column = 1; column < columns; column++)
        {
            double x = width * column / columns;
            entities.Add(new LineEntity(new LineSegment2D(new Point2D(x, 0.0), new Point2D(x, depth)), layerName: DefaultLayers.Score.Name));
        }

        for (int row = 1; row < rows; row++)
        {
            double y = depth * row / rows;
            entities.Add(new LineEntity(new LineSegment2D(new Point2D(0.0, y), new Point2D(width, y)), layerName: DefaultLayers.Score.Name));
        }

        return new Sketch(name: Name, entities: entities);
    }
}
