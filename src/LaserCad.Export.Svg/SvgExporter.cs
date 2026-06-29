using LaserCad.Core.Documents;
using LaserCad.Geometry;
using System.Globalization;
using System.Xml.Linq;

namespace LaserCad.Export.Svg;

/// <summary>
/// Eksportuje dokument CAD do tekstowej reprezentacji SVG.
/// </summary>
public sealed class SvgExporter
{
    private static readonly XNamespace SvgNamespace = "http://www.w3.org/2000/svg";

    /// <summary>
    /// Eksportuje dokument CAD do SVG zgodnie z podanymi opcjami.
    /// </summary>
    public string Export(CadDocument document, SvgExportOptions? options = null)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        options ??= new SvgExportOptions();

        BoundingBox bounds = CalculateBounds(document);

        var svg = new XElement(
            SvgNamespace + "svg",
            new XAttribute("xmlns", SvgNamespace.NamespaceName),
            new XAttribute("width", FormatMillimeters(bounds.Width)),
            new XAttribute("height", FormatMillimeters(bounds.Height)),
            new XAttribute("viewBox", FormatViewBox(bounds)),
            new XAttribute("fill", "none"),
            new XAttribute("stroke", "#000000"),
            new XAttribute("stroke-width", FormatNumber(options.StrokeWidthMillimeters)));

        foreach (Entity entity in document.Sketches.SelectMany(sketch => sketch.Entities))
        {
            var element = CreateElement(entity);

            if (element is not null)
            {
                svg.Add(element);
            }
        }

        return new XDocument(new XDeclaration("1.0", "utf-8", null), svg).ToString(SaveOptions.DisableFormatting);
    }

    private static XElement? CreateElement(Entity entity)
    {
        return entity switch
        {
            LineEntity line => CreateLineElement(line),
            RectangleEntity rectangle => CreateRectangleElement(rectangle),
            CircleEntity circle => CreateCircleElement(circle),
            _ => null,
        };
    }

    private static XElement CreateLineElement(LineEntity entity)
    {
        return new XElement(
            SvgNamespace + "line",
            new XAttribute("x1", FormatNumber(entity.Segment.Start.X)),
            new XAttribute("y1", FormatNumber(entity.Segment.Start.Y)),
            new XAttribute("x2", FormatNumber(entity.Segment.End.X)),
            new XAttribute("y2", FormatNumber(entity.Segment.End.Y)));
    }

    private static XElement CreateRectangleElement(RectangleEntity entity)
    {
        return new XElement(
            SvgNamespace + "path",
            new XAttribute("d", FormatClosedPath(entity.Corners)));
    }

    private static XElement CreateCircleElement(CircleEntity entity)
    {
        return new XElement(
            SvgNamespace + "circle",
            new XAttribute("cx", FormatNumber(entity.Circle.Center.X)),
            new XAttribute("cy", FormatNumber(entity.Circle.Center.Y)),
            new XAttribute("r", FormatNumber(entity.Circle.Radius)));
    }

    private static BoundingBox CalculateBounds(CadDocument document)
    {
        BoundingBox? bounds = null;

        foreach (Entity entity in document.Sketches.SelectMany(sketch => sketch.Entities))
        {
            bounds = bounds.HasValue ? bounds.Value.Union(entity.Bounds) : entity.Bounds;
        }

        return bounds ?? new BoundingBox(0, 0, 0, 0);
    }

    private static string FormatViewBox(BoundingBox bounds)
    {
        return string.Join(
            " ",
            FormatNumber(bounds.MinX),
            FormatNumber(bounds.MinY),
            FormatNumber(bounds.Width),
            FormatNumber(bounds.Height));
    }

    private static string FormatClosedPath(IReadOnlyList<Point2D> points)
    {
        return string.Concat(FormatOpenPath(points), " Z");
    }

    private static string FormatOpenPath(IReadOnlyList<Point2D> points)
    {
        var commands = new List<string>
        {
            string.Concat("M ", FormatPoint(points[0])),
        };

        for (int index = 1; index < points.Count; index++)
        {
            commands.Add(string.Concat("L ", FormatPoint(points[index])));
        }

        return string.Join(" ", commands);
    }

    private static string FormatPoint(Point2D point)
    {
        return string.Concat(FormatNumber(point.X), " ", FormatNumber(point.Y));
    }

    private static string FormatMillimeters(double value)
    {
        return string.Concat(FormatNumber(value), "mm");
    }

    private static string FormatNumber(double value)
    {
        return value.ToString("0.######", CultureInfo.InvariantCulture);
    }
}
