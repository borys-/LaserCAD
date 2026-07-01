using LaserCad.Core.Documents;
using LaserCad.Geometry;

namespace LaserCad.Core.Production;

/// <summary>
/// Analizuje dokument CAD pod katem podstawowych problemow produkcyjnych.
/// </summary>
public sealed class ManufacturingCheckAnalyzer
{
    private readonly ManufacturingCheckOptions options;

    /// <summary>
    /// Tworzy analizator kontroli produkcyjnych.
    /// </summary>
    public ManufacturingCheckAnalyzer(ManufacturingCheckOptions? options = null)
    {
        this.options = options ?? new ManufacturingCheckOptions();
    }

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
        AddSmallSpacingChecks(document, checks);
        AddThinBridgeChecks(document, checks);
        AddCutOrderSuggestion(document, checks);
        return checks;
    }

    private static void AddCutOrderSuggestion(CadDocument document, List<ManufacturingCheck> checks)
    {
        var closedContours = GetClosedContours(document)
            .OrderBy(contour => contour.Bounds.Width * contour.Bounds.Height)
            .ToArray();

        if (closedContours.Length < 2)
        {
            return;
        }

        checks.Add(new ManufacturingCheck(
            "CutOrderSuggestion",
            "Sugestia kolejnosci ciecia: najpierw mniejsze kontury wewnetrzne i detale, na koncu najwieksze obrysy zewnetrzne.",
            ManufacturingCheckSeverity.Info,
            closedContours[0].EntityId));
    }

    private void AddThinBridgeChecks(CadDocument document, List<ManufacturingCheck> checks)
    {
        var contours = GetClosedContours(document).ToArray();

        for (int i = 0; i < contours.Length; i++)
        {
            for (int j = i + 1; j < contours.Length; j++)
            {
                var bridgeWidth = GetAxisAlignedGap(contours[i].Bounds, contours[j].Bounds);
                if (bridgeWidth is null ||
                    bridgeWidth <= GeometryTolerance.Default ||
                    bridgeWidth >= options.MinimumBridgeWidth.Millimeters)
                {
                    continue;
                }

                checks.Add(new ManufacturingCheck(
                    "ThinBridge",
                    "Wykryto mostek materialu cienszy niz zalecane minimum.",
                    ManufacturingCheckSeverity.Warning,
                    contours[j].EntityId));
            }
        }
    }

    private void AddSmallSpacingChecks(CadDocument document, List<ManufacturingCheck> checks)
    {
        var segments = GetEntitySegments(document).ToArray();

        for (int i = 0; i < segments.Length; i++)
        {
            for (int j = i + 1; j < segments.Length; j++)
            {
                if (segments[i].EntityId == segments[j].EntityId)
                {
                    continue;
                }

                var distance = DistanceBetweenSegments(segments[i].Segment, segments[j].Segment);
                if (distance <= GeometryTolerance.Default || distance >= options.MinimumSpacing.Millimeters)
                {
                    continue;
                }

                checks.Add(new ManufacturingCheck(
                    "SmallSpacing",
                    "Wykryto odstep mniejszy niz zalecane minimum.",
                    ManufacturingCheckSeverity.Warning,
                    segments[j].EntityId));
            }
        }
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

    private static IEnumerable<EntityBounds> GetClosedContours(CadDocument document)
    {
        foreach (var sketch in document.Sketches)
        {
            foreach (var entity in sketch.Entities)
            {
                if (entity is RectangleEntity or CircleEntity)
                {
                    yield return new EntityBounds(entity.Id, entity.Bounds);
                }

                if (entity is PolylineEntity polyline && polyline.Polyline.IsClosed)
                {
                    yield return new EntityBounds(entity.Id, entity.Bounds);
                }
            }
        }
    }

    private static double? GetAxisAlignedGap(BoundingBox first, BoundingBox second)
    {
        var xOverlap = first.MinX <= second.MaxX && second.MinX <= first.MaxX;
        var yOverlap = first.MinY <= second.MaxY && second.MinY <= first.MaxY;

        if (yOverlap && first.MaxX < second.MinX)
        {
            return second.MinX - first.MaxX;
        }

        if (yOverlap && second.MaxX < first.MinX)
        {
            return first.MinX - second.MaxX;
        }

        if (xOverlap && first.MaxY < second.MinY)
        {
            return second.MinY - first.MaxY;
        }

        if (xOverlap && second.MaxY < first.MinY)
        {
            return first.MinY - second.MaxY;
        }

        return null;
    }

    private readonly record struct EntitySegment(Guid EntityId, LineSegment2D Segment);

    private readonly record struct EntityBounds(Guid EntityId, BoundingBox Bounds);

    private static double DistanceBetweenSegments(LineSegment2D first, LineSegment2D second)
    {
        var intersection = Intersections2D.Intersect(first, second);
        if (!intersection.IsNone && !intersection.IsParallel)
        {
            return 0.0;
        }

        return new[]
        {
            DistancePointToSegment(first.Start, second),
            DistancePointToSegment(first.End, second),
            DistancePointToSegment(second.Start, first),
            DistancePointToSegment(second.End, first),
        }.Min();
    }

    private static double DistancePointToSegment(Point2D point, LineSegment2D segment)
    {
        var segmentVector = segment.End - segment.Start;
        var lengthSquared = segmentVector.Dot(segmentVector);
        if (lengthSquared <= GeometryTolerance.Default)
        {
            return point.DistanceTo(segment.Start);
        }

        var pointVector = point - segment.Start;
        var t = Math.Max(0.0, Math.Min(1.0, pointVector.Dot(segmentVector) / lengthSquared));
        return point.DistanceTo(segment.PointAt(t));
    }

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
