using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Production;

/// <summary>
/// Analizuje dokument CAD pod katem podstawowych problemow produkcyjnych.
/// </summary>
public sealed class ManufacturingCheckAnalyzer
{
    /// <summary>
    /// Zwraca liste wynikow kontroli produkcyjnych dla dokumentu.
    /// </summary>
    public IReadOnlyList<ManufacturingCheck> Analyze(CadDocument document)
    {
        if (document is null)
        {
            throw new ArgumentNullException(nameof(document));
        }

        var checks = new List<ManufacturingCheck>();
        AddDuplicateLineChecks(document, checks);
        AddOpenContourChecks(document, checks);
        return checks;
    }

    private static void AddOpenContourChecks(CadDocument document, List<ManufacturingCheck> checks)
    {
        foreach (var sketch in document.Sketches)
        {
            foreach (var polyline in sketch.Entities.OfType<PolylineEntity>())
            {
                if (polyline.Polyline.IsClosed)
                {
                    continue;
                }

                checks.Add(new ManufacturingCheck(
                    "OpenContour",
                    "Wykryto otwarty kontur polilinii.",
                    ManufacturingCheckSeverity.Warning,
                    polyline.Id));
            }
        }
    }

    private static void AddDuplicateLineChecks(CadDocument document, List<ManufacturingCheck> checks)
    {
        var seenSegments = new Dictionary<SegmentKey, EntitySegment>();

        foreach (var segment in GetEntitySegments(document))
        {
            var key = SegmentKey.Create(segment.Segment);
            if (seenSegments.ContainsKey(key))
            {
                checks.Add(new ManufacturingCheck(
                    "DuplicateLine",
                    "Wykryto podwojna linie ciecia.",
                    ManufacturingCheckSeverity.Error,
                    segment.EntityId));
                continue;
            }

            seenSegments.Add(key, segment);
        }
    }

    private static IEnumerable<EntitySegment> GetEntitySegments(CadDocument document)
    {
        foreach (var sketch in document.Sketches)
        {
            foreach (var entity in sketch.Entities)
            {
                foreach (var segment in GetEntitySegments(entity))
                {
                    yield return segment;
                }
            }
        }
    }

    private static IEnumerable<EntitySegment> GetEntitySegments(Entity entity)
    {
        switch (entity)
        {
            case LineEntity line:
                yield return new EntitySegment(entity.Id, line.Segment);
                break;
            case RectangleEntity rectangle:
                for (int i = 0; i < rectangle.Corners.Count; i++)
                {
                    yield return new EntitySegment(
                        entity.Id,
                        new LineSegment2D(rectangle.Corners[i], rectangle.Corners[(i + 1) % rectangle.Corners.Count]));
                }

                break;
            case PolylineEntity polyline:
                for (int i = 0; i < polyline.Polyline.Points.Count - 1; i++)
                {
                    yield return new EntitySegment(
                        entity.Id,
                        new LineSegment2D(polyline.Polyline.Points[i], polyline.Polyline.Points[i + 1]));
                }

                if (polyline.Polyline.IsClosed)
                {
                    yield return new EntitySegment(
                        entity.Id,
                        new LineSegment2D(polyline.Polyline.Points[^1], polyline.Polyline.Points[0]));
                }

                break;
        }
    }

    private readonly record struct EntitySegment(Guid EntityId, LineSegment2D Segment);

    private readonly record struct SegmentKey(long StartX, long StartY, long EndX, long EndY)
    {
        public static SegmentKey Create(LineSegment2D segment)
        {
            var startX = Quantize(segment.Start.X);
            var startY = Quantize(segment.Start.Y);
            var endX = Quantize(segment.End.X);
            var endY = Quantize(segment.End.Y);

            if (ComparePoints(startX, startY, endX, endY) > 0)
            {
                return new SegmentKey(endX, endY, startX, startY);
            }

            return new SegmentKey(startX, startY, endX, endY);
        }

        private static long Quantize(double value)
        {
            return (long)Math.Round(value / GeometryTolerance.Default);
        }

        private static int ComparePoints(long leftX, long leftY, long rightX, long rightY)
        {
            var xComparison = leftX.CompareTo(rightX);
            return xComparison != 0 ? xComparison : leftY.CompareTo(rightY);
        }
    }
}
