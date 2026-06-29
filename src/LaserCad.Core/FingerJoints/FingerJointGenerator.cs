using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.FingerJoints;

/// <summary>
/// Generator profilu polaczenia palcowego dla prostych krawedzi.
/// </summary>
public sealed class FingerJointGenerator
{
    /// <summary>
    /// Generuje profil polaczenia palcowego dla podanej krawedzi.
    /// Normalna zewnetrzna lezy po lewej stronie kierunku krawedzi.
    /// </summary>
    public FingerJointProfile GenerateEdge(LineSegment2D edge, Length materialThickness, FingerJointOptions? options = null)
    {
        if (edge.Length <= GeometryTolerance.Default)
        {
            throw new ArgumentException("Finger joint edge must have positive length.", nameof(edge));
        }

        if (materialThickness <= Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(nameof(materialThickness), "Material thickness must be greater than zero.");
        }

        FingerJointOptions effectiveOptions = options ?? new FingerJointOptions();
        int segmentCount = ChooseSegmentCount(edge.Length, effectiveOptions);
        Length segmentLength = Length.FromMillimeters(edge.Length / segmentCount);
        var segments = BuildSegments(edge, segmentCount, segmentLength, effectiveOptions);
        Length kerfCompensation = effectiveOptions.Kerf / 2.0;
        Length clearanceCompensation = CalculateClearanceCompensation(effectiveOptions);
        Length slotInset = kerfCompensation + clearanceCompensation;
        Length outwardDepth = materialThickness + kerfCompensation - clearanceCompensation;

        if (outwardDepth <= Length.FromMillimeters(0.0))
        {
            throw new ArgumentException("Finger joint compensation makes outward depth non-positive.", nameof(options));
        }

        IReadOnlyList<Point2D> points = BuildProfilePoints(edge, segments, outwardDepth, slotInset);

        return new FingerJointProfile(
            segments,
            points,
            materialThickness,
            outwardDepth,
            slotInset,
            kerfCompensation,
            clearanceCompensation);
    }

    private static int ChooseSegmentCount(double edgeLength, FingerJointOptions options)
    {
        int nearest = Math.Max(1, (int)Math.Round(edgeLength / options.FingerWidth.Millimeters));
        int bestCount = 0;
        double bestScore = double.MaxValue;

        for (int count = 1; count <= Math.Max(1, (int)Math.Ceiling(edgeLength / options.MinimumFingerWidth.Millimeters)); count++)
        {
            double segmentLength = edgeLength / count;
            if (segmentLength + GeometryTolerance.Default < options.MinimumFingerWidth.Millimeters ||
                segmentLength - GeometryTolerance.Default > options.MaximumFingerWidth.Millimeters)
            {
                continue;
            }

            if (!CanSatisfyEdgeKinds(count, options))
            {
                continue;
            }

            double score = Math.Abs(segmentLength - options.FingerWidth.Millimeters);
            if (score < bestScore)
            {
                bestScore = score;
                bestCount = count;
            }
        }

        if (bestCount == 0)
        {
            throw new ArgumentException("Cannot divide edge into valid finger joint segments with current options.", nameof(options));
        }

        return bestCount;
    }

    private static bool CanSatisfyEdgeKinds(int count, FingerJointOptions options)
    {
        if (options.StartWithFinger == options.EndWithFinger)
        {
            return count % 2 == 1;
        }

        return count % 2 == 0;
    }

    private static IReadOnlyList<FingerJointSegment> BuildSegments(
        LineSegment2D edge,
        int segmentCount,
        Length segmentLength,
        FingerJointOptions options)
    {
        var segments = new FingerJointSegment[segmentCount];

        for (int index = 0; index < segmentCount; index++)
        {
            double startT = (double)index / segmentCount;
            double endT = (double)(index + 1) / segmentCount;
            FingerJointSegmentKind kind = IsFinger(index, options)
                ? FingerJointSegmentKind.Finger
                : FingerJointSegmentKind.Slot;

            segments[index] = new FingerJointSegment(
                kind,
                Length.FromMillimeters(segmentLength.Millimeters * index),
                segmentLength,
                edge.PointAt(startT),
                edge.PointAt(endT));
        }

        return segments;
    }

    private static bool IsFinger(int index, FingerJointOptions options)
    {
        bool evenSegment = index % 2 == 0;

        return options.StartWithFinger ? evenSegment : !evenSegment;
    }

    private static Length CalculateClearanceCompensation(FingerJointOptions options)
    {
        return options.FitMode switch
        {
            FingerJointFitMode.Tight => Length.FromMillimeters(0.0),
            FingerJointFitMode.Neutral => options.Clearance / 2.0,
            FingerJointFitMode.Loose => options.Clearance,
            _ => throw new ArgumentOutOfRangeException(nameof(options), "Unknown finger joint fit mode.")
        };
    }

    private static IReadOnlyList<Point2D> BuildProfilePoints(
        LineSegment2D edge,
        IReadOnlyList<FingerJointSegment> segments,
        Length outwardDepth,
        Length slotInset)
    {
        Vector2D direction = edge.Direction;
        Vector2D outward = new Vector2D(-direction.Y, direction.X);
        var points = new List<Point2D> { edge.Start };
        FingerJointSegmentKind? previousKind = null;

        foreach (FingerJointSegment segment in segments)
        {
            Vector2D offset = OffsetFor(segment.Kind, outward, outwardDepth, slotInset);

            if (previousKind != segment.Kind)
            {
                points.Add(segment.Start + offset);
            }

            points.Add(segment.End + offset);
            previousKind = segment.Kind;
        }

        points.Add(edge.End);

        return points;
    }

    private static Vector2D OffsetFor(
        FingerJointSegmentKind kind,
        Vector2D outward,
        Length outwardDepth,
        Length slotInset)
    {
        double distance = kind == FingerJointSegmentKind.Finger
            ? outwardDepth.Millimeters
            : -slotInset.Millimeters;

        return new Vector2D(outward.X * distance, outward.Y * distance);
    }
}
