using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Kerf;

/// <summary>
/// Generuje prosty probnik kalibracyjny do wyciecia i porownania szczelin.
/// </summary>
public sealed class KerfCalibrationGenerator
{
    /// <summary>
    /// Generuje szkic probnika z ramka i miejscem na szczeliny testowe.
    /// </summary>
    public Sketch GenerateSketch(KerfCalibrationOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        var totalWidth = (options.Margin.Millimeters * 2.0)
            + (options.SlotWidth.Millimeters * options.SlotCount)
            + (options.Spacing.Millimeters * (options.SlotCount - 1));
        var totalHeight = (options.Margin.Millimeters * 2.0) + options.SlotHeight.Millimeters;
        var sketch = new Sketch(name: "Probnik kerfu")
            .AddEntity(new RectangleEntity(new Point2D(0.0, 0.0), totalWidth, totalHeight, layerName: DefaultLayers.Cut.Name));

        var slotY = options.Margin.Millimeters;
        for (var i = 0; i < options.SlotCount; i++)
        {
            var slotX = options.Margin.Millimeters + (i * (options.SlotWidth.Millimeters + options.Spacing.Millimeters));
            sketch = sketch.AddEntity(
                new RectangleEntity(
                    new Point2D(slotX, slotY),
                    options.SlotWidth.Millimeters,
                    options.SlotHeight.Millimeters,
                    layerName: DefaultLayers.Cut.Name));
        }

        return sketch;
    }
}
