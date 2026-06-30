using LaserCad.Geometry;
using LaserCad.Core.Parameters;

namespace LaserCad.Core.Documents;

/// <summary>
/// Encja tekstowa szkicu.
/// W MVP przechowuje tekst, pozycje, rozmiar, font i wyrownanie bez zaleznosci od silnika fontow.
/// </summary>
public sealed class TextEntity : Entity
{
    /// <summary>
    /// Tworzy encje tekstowa.
    /// </summary>
    public TextEntity(
        string text,
        Point2D position,
        double height,
        Guid? id = null,
        string layerName = "Engrave",
        string fontFamily = "Arial",
        TextAlignment alignment = TextAlignment.Left,
        string? fontFilePath = null)
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
        Font = new TextFontSource(fontFamily, fontFilePath);
        Alignment = alignment;
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

    /// <summary>
    /// Informacja o foncie tekstu.
    /// </summary>
    public TextFontSource Font { get; }

    /// <summary>
    /// Wyrownanie tekstu wzgledem punktu bazowego.
    /// </summary>
    public TextAlignment Alignment { get; }

    /// <inheritdoc />
    public override BoundingBox Bounds
    {
        get
        {
            double width = EstimateWidth();
            double minX = Alignment switch
            {
                TextAlignment.Center => Position.X - width / 2.0,
                TextAlignment.Right => Position.X - width,
                _ => Position.X
            };

            return new BoundingBox(minX, Position.Y, minX + width, Position.Y + Height);
        }
    }

    /// <inheritdoc />
    public override ISketchEntity Transform(Matrix3x3 transform)
    {
        Point2D transformedPosition = transform.Transform(Position);
        double transformedHeight = transform.Transform(new Vector2D(0.0, Height)).Length;

        return new TextEntity(Text, transformedPosition, transformedHeight, Id, LayerName, Font.FamilyName, Alignment, Font.FilePath);
    }

    /// <inheritdoc />
    public override Entity Copy(Guid? id = null)
    {
        return new TextEntity(Text, Position, Height, id, LayerName, Font.FamilyName, Alignment, Font.FilePath);
    }

    /// <summary>
    /// Zwraca kopie z podmieniona rodzina fontu.
    /// </summary>
    public TextEntity WithFontFamily(string fontFamily)
    {
        return new TextEntity(Text, Position, Height, Id, LayerName, fontFamily, Alignment, Font.FilePath);
    }

    /// <summary>
    /// Zwraca kopie z informacja o zaimportowanym pliku fontu.
    /// </summary>
    public TextEntity WithImportedFont(string fontFamily, string fontFilePath)
    {
        return new TextEntity(Text, Position, Height, Id, LayerName, fontFamily, Alignment, fontFilePath);
    }

    /// <summary>
    /// Zwraca kopie z podmienionym wyrownaniem.
    /// </summary>
    public TextEntity WithAlignment(TextAlignment alignment)
    {
        return new TextEntity(Text, Position, Height, Id, LayerName, Font.FamilyName, alignment, Font.FilePath);
    }

    /// <summary>
    /// Podmienia znaczniki parametrow w formacie {Id} wartosciami z zestawu parametrow.
    /// </summary>
    public TextEntity ResolveParameters(ParameterSet parameters)
    {
        if (parameters is null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        string resolvedText = Text;

        foreach (Parameter parameter in parameters.Parameters)
        {
            resolvedText = resolvedText.Replace(
                "{" + parameter.Id.Value + "}",
                Convert.ToString(parameter.Value, System.Globalization.CultureInfo.InvariantCulture),
                StringComparison.Ordinal);
        }

        return new TextEntity(resolvedText, Position, Height, Id, LayerName, Font.FamilyName, Alignment, Font.FilePath);
    }

    /// <summary>
    /// Szacuje szerokosc tekstu dla potrzeb bounding boxa MVP.
    /// </summary>
    public double EstimateWidth()
    {
        return Text.Length * Height * 0.6;
    }
}
