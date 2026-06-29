using LaserCad.Geometry;

namespace LaserCad.Tests.Geometry;

public sealed class Circle2DTests
{
    [Test]
    public void Constructor_ShouldExposeCenterAndRadius()
    {
        Circle2D circle = new Circle2D(new Point2D(2.0, 3.0), 5.0);

        Assert.Multiple(() =>
        {
            Assert.That(circle.Center, Is.EqualTo(new Point2D(2.0, 3.0)));
            Assert.That(circle.Radius, Is.EqualTo(5.0));
        });
    }

    [Test]
    public void Constructor_WithNonPositiveRadius_ShouldThrow()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Circle2D(new Point2D(0.0, 0.0), 0.0));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Circle2D(new Point2D(0.0, 0.0), -1.0));
        });
    }

    [Test]
    public void Circumference_ShouldReturnCircleCircumference()
    {
        Circle2D circle = new Circle2D(new Point2D(0.0, 0.0), 4.0);

        Assert.That(circle.Circumference, Is.EqualTo(8.0 * Math.PI).Within(GeometryTolerance.Default));
    }

    [Test]
    public void Bounds_ShouldReturnBoxAroundCircle()
    {
        Circle2D circle = new Circle2D(new Point2D(2.0, 3.0), 5.0);

        Assert.That(circle.Bounds, Is.EqualTo(new BoundingBox(-3.0, -2.0, 7.0, 8.0)));
    }

    [Test]
    public void Transform_ShouldTransformCenterAndRadius()
    {
        Circle2D circle = new Circle2D(new Point2D(1.0, 2.0), 3.0);
        Matrix3x3 transform = Matrix3x3.CreateTranslation(10.0, -2.0)
            * Matrix3x3.CreateRotation(Math.PI / 2.0)
            * Matrix3x3.CreateScaling(2.0, 2.0);

        Circle2D result = circle.Transform(transform);

        Assert.Multiple(() =>
        {
            Assert.That(result.Center.X, Is.EqualTo(6.0).Within(GeometryTolerance.Default));
            Assert.That(result.Center.Y, Is.EqualTo(0.0).Within(GeometryTolerance.Default));
            Assert.That(result.Radius, Is.EqualTo(6.0).Within(GeometryTolerance.Default));
        });
    }

    [Test]
    public void Transform_WithNonUniformScaling_ShouldThrow()
    {
        Circle2D circle = new Circle2D(new Point2D(1.0, 2.0), 3.0);

        Assert.Throws<InvalidOperationException>(() => circle.Transform(Matrix3x3.CreateScaling(2.0, 3.0)));
    }
}
