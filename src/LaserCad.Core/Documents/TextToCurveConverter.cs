using LaserCad.Geometry;

namespace LaserCad.Core.Documents;

/// <summary>
/// Konwertuje tekst na uproszczone krzywe MVP.
/// </summary>
public sealed class TextToCurveConverter
{
    /// <summary>
    /// Zamienia kazdy znak tekstu na prostokatny obrys przyblizajacy pole znaku.
    /// </summary>
    public IReadOnlyList<PolylineEntity> ConvertToCurves(TextEntity textEntity)
    {
        if (textEntity is null)
        {
            throw new ArgumentNullException(nameof(textEntity));
        }

        double characterWidth = textEntity.Height * 0.6;
        double totalWidth = textEntity.EstimateWidth();
        double x = textEntity.Alignment switch
        {
            TextAlignment.Center => textEntity.Position.X - totalWidth / 2.0,
            TextAlignment.Right => textEntity.Position.X - totalWidth,
            _ => textEntity.Position.X
        };

        var curves = new List<PolylineEntity>();

        foreach (char character in textEntity.Text)
        {
            if (!char.IsWhiteSpace(character))
            {
                curves.Add(CreateCharacterBox(textEntity, x, characterWidth));
            }

            x += characterWidth;
        }

        return curves;
    }

    private static PolylineEntity CreateCharacterBox(TextEntity textEntity, double x, double characterWidth)
    {
        var y = textEntity.Position.Y;
        var points = new[]
        {
            new Point2D(x, y),
            new Point2D(x + characterWidth, y),
            new Point2D(x + characterWidth, y + textEntity.Height),
            new Point2D(x, y + textEntity.Height),
        };

        return new PolylineEntity(new Polyline2D(points, isClosed: true), layerName: textEntity.LayerName);
    }
}
