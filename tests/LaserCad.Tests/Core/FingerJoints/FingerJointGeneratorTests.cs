using LaserCad.Core.FingerJoints;
using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Tests.Core.FingerJoints;

public sealed class FingerJointGeneratorTests
{
    [Test]
    public void GenerateEdge_WithDefaultOptions_ShouldCreateProfile()
    {
        var generator = new FingerJointGenerator();
        var edge = new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(11.0, 0.0));

        FingerJointProfile profile = generator.GenerateEdge(edge, Length.FromMillimeters(3.0));

        Assert.That(profile.Segments, Has.Count.EqualTo(11));
        Assert.That(profile.Segments[0].Kind, Is.EqualTo(FingerJointSegmentKind.Finger));
        Assert.That(profile.Segments[^1].Kind, Is.EqualTo(FingerJointSegmentKind.Finger));
        Assert.That(profile.Points.First(), Is.EqualTo(edge.Start));
        Assert.That(profile.Points.Last(), Is.EqualTo(edge.End));
    }

    [Test]
    public void GenerateEdge_WithHundredMillimeterEdge_ShouldDivideIntoValidSegments()
    {
        var generator = new FingerJointGenerator();
        var options = new FingerJointOptions(
            fingerWidth: Length.FromMillimeters(10.0),
            minimumFingerWidth: Length.FromMillimeters(8.0),
            maximumFingerWidth: Length.FromMillimeters(12.0));
        var edge = new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(100.0, 0.0));

        FingerJointProfile profile = generator.GenerateEdge(edge, Length.FromMillimeters(3.0), options);

        Assert.That(profile.Segments, Has.Count.EqualTo(11));
        Assert.That(profile.Segments.All(segment => segment.Length.Millimeters >= 8.0), Is.True);
        Assert.That(profile.Segments.All(segment => segment.Length.Millimeters <= 12.0), Is.True);
        Assert.That(profile.Segments.Sum(segment => segment.Length.Millimeters), Is.EqualTo(100.0).Within(1e-9));
    }

    [Test]
    public void GenerateEdge_WithSameStartAndEndKind_ShouldUseSymmetricOddSegmentCount()
    {
        var generator = new FingerJointGenerator();
        var options = new FingerJointOptions(
            fingerWidth: Length.FromMillimeters(10.0),
            minimumFingerWidth: Length.FromMillimeters(5.0),
            maximumFingerWidth: Length.FromMillimeters(20.0),
            startWithFinger: true,
            endWithFinger: true);
        var edge = new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(100.0, 0.0));

        FingerJointProfile profile = generator.GenerateEdge(edge, Length.FromMillimeters(3.0), options);

        Assert.That(profile.Segments.Count % 2, Is.EqualTo(1));
        Assert.That(profile.Segments[0].Kind, Is.EqualTo(profile.Segments[^1].Kind));
        Assert.That(profile.Segments[0].Length, Is.EqualTo(profile.Segments[^1].Length));
    }

    [Test]
    public void GenerateEdge_WithStartWithFinger_ShouldStartWithFinger()
    {
        var generator = new FingerJointGenerator();
        var options = new FingerJointOptions(
            fingerWidth: Length.FromMillimeters(10.0),
            minimumFingerWidth: Length.FromMillimeters(5.0),
            maximumFingerWidth: Length.FromMillimeters(20.0),
            startWithFinger: true,
            endWithFinger: false);
        var edge = new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(100.0, 0.0));

        FingerJointProfile profile = generator.GenerateEdge(edge, Length.FromMillimeters(3.0), options);

        Assert.That(profile.Segments[0].Kind, Is.EqualTo(FingerJointSegmentKind.Finger));
        Assert.That(profile.Segments[^1].Kind, Is.EqualTo(FingerJointSegmentKind.Slot));
    }

    [Test]
    public void GenerateEdge_WithStartWithSlot_ShouldStartWithSlot()
    {
        var generator = new FingerJointGenerator();
        var options = new FingerJointOptions(
            fingerWidth: Length.FromMillimeters(10.0),
            minimumFingerWidth: Length.FromMillimeters(5.0),
            maximumFingerWidth: Length.FromMillimeters(20.0),
            startWithFinger: false,
            endWithFinger: true);
        var edge = new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(100.0, 0.0));

        FingerJointProfile profile = generator.GenerateEdge(edge, Length.FromMillimeters(3.0), options);

        Assert.That(profile.Segments[0].Kind, Is.EqualTo(FingerJointSegmentKind.Slot));
        Assert.That(profile.Segments[^1].Kind, Is.EqualTo(FingerJointSegmentKind.Finger));
    }

    [Test]
    public void GenerateEdge_ShouldUseMaterialThicknessForFingerDepth()
    {
        var generator = new FingerJointGenerator();
        var options = new FingerJointOptions(
            fingerWidth: Length.FromMillimeters(10.0),
            minimumFingerWidth: Length.FromMillimeters(5.0),
            maximumFingerWidth: Length.FromMillimeters(20.0));
        var edge = new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(100.0, 0.0));

        FingerJointProfile profile = generator.GenerateEdge(edge, Length.FromMillimeters(4.0), options);

        Assert.That(profile.OutwardDepth, Is.EqualTo(Length.FromMillimeters(4.0)));
        Assert.That(profile.Points.Any(point => point.Y > 3.9), Is.True);
    }

    [Test]
    public void GenerateEdge_ShouldApplyKerfCompensation()
    {
        var generator = new FingerJointGenerator();
        var options = new FingerJointOptions(
            fingerWidth: Length.FromMillimeters(10.0),
            minimumFingerWidth: Length.FromMillimeters(5.0),
            maximumFingerWidth: Length.FromMillimeters(20.0),
            kerf: Length.FromMillimeters(0.2));
        var edge = new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(100.0, 0.0));

        FingerJointProfile profile = generator.GenerateEdge(edge, Length.FromMillimeters(3.0), options);

        Assert.That(profile.KerfCompensation, Is.EqualTo(Length.FromMillimeters(0.1)));
        Assert.That(profile.OutwardDepth, Is.EqualTo(Length.FromMillimeters(3.1)));
        Assert.That(profile.SlotInset, Is.EqualTo(Length.FromMillimeters(0.1)));
    }

    [Test]
    public void GenerateEdge_ShouldApplyClearanceCompensation()
    {
        var generator = new FingerJointGenerator();
        var options = new FingerJointOptions(
            fingerWidth: Length.FromMillimeters(10.0),
            minimumFingerWidth: Length.FromMillimeters(5.0),
            maximumFingerWidth: Length.FromMillimeters(20.0),
            fitMode: FingerJointFitMode.Loose,
            clearance: Length.FromMillimeters(0.2));
        var edge = new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(100.0, 0.0));

        FingerJointProfile profile = generator.GenerateEdge(edge, Length.FromMillimeters(3.0), options);

        Assert.That(profile.ClearanceCompensation, Is.EqualTo(Length.FromMillimeters(0.2)));
        Assert.That(profile.OutwardDepth, Is.EqualTo(Length.FromMillimeters(2.8)));
        Assert.That(profile.SlotInset, Is.EqualTo(Length.FromMillimeters(0.2)));
    }
}
