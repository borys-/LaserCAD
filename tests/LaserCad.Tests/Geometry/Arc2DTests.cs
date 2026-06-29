using LaserCad.Geometry;

namespace LaserCad.Tests.Geometry;

public sealed class Arc2DTests
{
    [Test]
    public void Constructor_ShouldExposeArcProperties()
    {
        Arc2D arc = new Arc2D(new Point2D(2.0, 3.0), 5.0, 0.25, 1.5, ArcDirection.Clockwise);

        Assert.Multiple(() =>
        {
            Assert.That(arc.Center, Is.EqualTo(new Point2D(2.0, 3.0)));
            Assert.That(arc.Radius, Is.EqualTo(5.0));
            Assert.That(arc.StartAngleRadians, Is.EqualTo(0.25));
            Assert.That(arc.EndAngleRadians, Is.EqualTo(1.5));
            Assert.That(arc.Direction, Is.EqualTo(ArcDirection.Clockwise));
        });
    }

    [Test]
    public void Constructor_ShouldUseCounterclockwiseDirectionByDefault()
    {
        Arc2D arc = new Arc2D(new Point2D(0.0, 0.0), 5.0, 0.0, Math.PI);

        Assert.That(arc.Direction, Is.EqualTo(ArcDirection.Counterclockwise));
    }

    [Test]
    public void Constructor_WithNonPositiveRadius_ShouldThrow()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Arc2D(new Point2D(0.0, 0.0), 0.0, 0.0, Math.PI));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Arc2D(new Point2D(0.0, 0.0), -1.0, 0.0, Math.PI));
        });
    }

    [Test]
    public void Length_ShouldReturnCounterclockwiseArcLength()
    {
        Arc2D arc = new Arc2D(new Point2D(0.0, 0.0), 4.0, 0.0, Math.PI / 2.0);

        Assert.That(arc.Length, Is.EqualTo(2.0 * Math.PI).Within(GeometryTolerance.Default));
    }

    [Test]
    public void Length_ShouldReturnClockwiseArcLength()
    {
        Arc2D arc = new Arc2D(new Point2D(0.0, 0.0), 4.0, 0.0, Math.PI / 2.0, ArcDirection.Clockwise);

        Assert.That(arc.Length, Is.EqualTo(6.0 * Math.PI).Within(GeometryTolerance.Default));
    }

    [Test]
    public void PointAt_ShouldReturnPointOnCounterclockwiseArc()
    {
        Arc2D arc = new Arc2D(new Point2D(1.0, 2.0), 4.0, 0.0, Math.PI);

        Point2D result = arc.PointAt(0.5);

        Assert.Multiple(() =>
        {
            Assert.That(result.X, Is.EqualTo(1.0).Within(GeometryTolerance.Default));
            Assert.That(result.Y, Is.EqualTo(6.0).Within(GeometryTolerance.Default));
        });
    }

    [Test]
    public void PointAt_ShouldReturnPointOnClockwiseArc()
    {
        Arc2D arc = new Arc2D(new Point2D(1.0, 2.0), 4.0, 0.0, Math.PI, ArcDirection.Clockwise);

        Point2D result = arc.PointAt(0.5);

        Assert.Multiple(() =>
        {
            Assert.That(result.X, Is.EqualTo(1.0).Within(GeometryTolerance.Default));
            Assert.That(result.Y, Is.EqualTo(-2.0).Within(GeometryTolerance.Default));
        });
    }

    [Test]
    public void PointAt_ShouldReturnEndpointsForZeroAndOne()
    {
        Arc2D arc = new Arc2D(new Point2D(1.0, 2.0), 4.0, 0.0, Math.PI / 2.0);

        Assert.Multiple(() =>
        {
            Assert.That(arc.PointAt(0.0), Is.EqualTo(new Point2D(5.0, 2.0)));
            Assert.That(arc.PointAt(1.0).X, Is.EqualTo(1.0).Within(GeometryTolerance.Default));
            Assert.That(arc.PointAt(1.0).Y, Is.EqualTo(6.0).Within(GeometryTolerance.Default));
        });
    }

    [Test]
    public void PointAt_WithParameterOutsideRange_ShouldThrow()
    {
        Arc2D arc = new Arc2D(new Point2D(0.0, 0.0), 4.0, 0.0, Math.PI);

        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => arc.PointAt(-0.1));
            Assert.Throws<ArgumentOutOfRangeException>(() => arc.PointAt(1.1));
        });
    }

    [Test]
    public void Transform_ShouldTransformArcAndKeepDirectionForRotation()
    {
        Arc2D arc = new Arc2D(new Point2D(1.0, 2.0), 3.0, 0.0, Math.PI / 2.0);
        Matrix3x3 transform = Matrix3x3.CreateTranslation(10.0, 0.0) * Matrix3x3.CreateRotation(Math.PI / 2.0);

        Arc2D result = arc.Transform(transform);

        Assert.Multiple(() =>
        {
            Assert.That(result.Center.X, Is.EqualTo(8.0).Within(GeometryTolerance.Default));
            Assert.That(result.Center.Y, Is.EqualTo(1.0).Within(GeometryTolerance.Default));
            Assert.That(result.Radius, Is.EqualTo(3.0).Within(GeometryTolerance.Default));
            Assert.That(result.StartAngleRadians, Is.EqualTo(Math.PI / 2.0).Within(GeometryTolerance.Default));
            Assert.That(result.EndAngleRadians, Is.EqualTo(Math.PI).Within(GeometryTolerance.Default));
            Assert.That(result.Direction, Is.EqualTo(ArcDirection.Counterclockwise));
        });
    }

    [Test]
    public void Transform_ShouldReverseDirectionForReflection()
    {
        Arc2D arc = new Arc2D(new Point2D(0.0, 0.0), 3.0, 0.0, Math.PI / 2.0);

        Arc2D result = arc.Transform(Matrix3x3.CreateReflectionY());

        Assert.Multiple(() =>
        {
            Assert.That(result.Direction, Is.EqualTo(ArcDirection.Clockwise));
            Assert.That(result.StartAngleRadians, Is.EqualTo(Math.PI).Within(GeometryTolerance.Default));
            Assert.That(result.EndAngleRadians, Is.EqualTo(Math.PI / 2.0).Within(GeometryTolerance.Default));
        });
    }

    [Test]
    public void Transform_WithNonUniformScaling_ShouldThrow()
    {
        Arc2D arc = new Arc2D(new Point2D(0.0, 0.0), 3.0, 0.0, Math.PI / 2.0);

        Assert.Throws<InvalidOperationException>(() => arc.Transform(Matrix3x3.CreateScaling(2.0, 3.0)));
    }
}
