using LaserCad.Geometry;

namespace LaserCad.Core.Documents;

/// <summary>
/// Placeholder encji tekstowej.
/// W MVP przechowuje tekst, pozycje i przyblizony rozmiar bez zaleznosci od silnika fontow.
/// </summary>
public sealed class TextEntity : Entity
{
    /// <summary>
    /// Tworzy encje tekstowa placeholder.
    /// </summary>
    public TextEntity(string text, Point2D position, double height, Guid? id = null, string layerName = "Engrave")
        : base(id, layerName)
    {
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Text cannot be empty.", nameof(text));
        }

        if (height <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "Text height must be positive.");
        }

        Text = text;
        Position = position;
        Height = height;
    }

    /// <summary>
    /// Tresc tekstu.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Punkt bazowy tekstu.
    /// </summary>
    public Point2D Position { get; }

    /// <summary>
    /// Przyblizona wysokosc tekstu w milimetrach domenowych.
    /// </summary>
    public double Height { get; }

    /// <inheritdoc />
    public override BoundingBox Bounds => new(Position.X, Position.Y, Position.X, Position.Y + Height);

    /// <inheritdoc />
    public override ISketchEntity Transform(Matrix3x3 transform)
    {
        Point2D transformedPosition = transform.Transform(Position);
        double transformedHeight = transform.Transform(new Vector2D(0.0, Height)).Length;

        return new TextEntity(Text, transformedPosition, transformedHeight, Id, LayerName);
    }

    /// <inheritdoc />
    public override Entity Copy(Guid? id = null)
    {
        return new TextEntity(Text, Position, Height, id, LayerName);
    }
}
