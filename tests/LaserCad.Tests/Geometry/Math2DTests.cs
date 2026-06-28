using LaserCad.Geometry;

namespace LaserCad.Tests.Geometry;

public sealed class Math2DTests
{
    [Test]
    public void Point2D_ShouldExposeCoordinates()
    {
        Point2D point = new Point2D(2.0, 3.0);

        Assert.Multiple(() =>
        {
            Assert.That(point.X, Is.EqualTo(2.0));
            Assert.That(point.Y, Is.EqualTo(3.0));
        });
    }

    [Test]
    public void Point2D_AddVector_ShouldTranslatePoint()
    {
        Point2D result = new Point2D(2.0, 3.0) + new Vector2D(4.0, -1.0);

        Assert.That(result, Is.EqualTo(new Point2D(6.0, 2.0)));
    }

    [Test]
    public void Point2D_DistanceTo_ShouldReturnDistanceBetweenPoints()
    {
        double distance = new Point2D(1.0, 2.0).DistanceTo(new Point2D(4.0, 6.0));

        Assert.That(distance, Is.EqualTo(5.0).Within(GeometryTolerance.Default));
    }

    [Test]
    public void Vector2D_ShouldExposeCoordinates()
    {
        Vector2D vector = new Vector2D(2.0, 3.0);

        Assert.Multiple(() =>
        {
            Assert.That(vector.X, Is.EqualTo(2.0));
            Assert.That(vector.Y, Is.EqualTo(3.0));
        });
    }

    [Test]
    public void Vector2D_Length_ShouldReturnVectorLength()
    {
        Vector2D vector = new Vector2D(3.0, 4.0);

        Assert.That(vector.Length, Is.EqualTo(5.0).Within(GeometryTolerance.Default));
    }

    [Test]
    public void Vector2D_Normalize_ShouldReturnUnitVector()
    {
        Vector2D normalized = new Vector2D(3.0, 4.0).Normalize();

        Assert.Multiple(() =>
        {
            Assert.That(normalized.X, Is.EqualTo(0.6).Within(GeometryTolerance.Default));
            Assert.That(normalized.Y, Is.EqualTo(0.8).Within(GeometryTolerance.Default));
            Assert.That(normalized.Length, Is.EqualTo(1.0).Within(GeometryTolerance.Default));
        });
    }

    [Test]
    public void Vector2D_NormalizeZeroVector_ShouldThrow()
    {
        Vector2D vector = new Vector2D(0.0, 0.0);

        Assert.Throws<InvalidOperationException>(() => vector.Normalize());
    }

    [Test]
    public void Vector2D_Dot_ShouldReturnDotProduct()
    {
        double dot = new Vector2D(2.0, 3.0).Dot(new Vector2D(4.0, 5.0));

        Assert.That(dot, Is.EqualTo(23.0));
    }

    [Test]
    public void Vector2D_Cross_ShouldReturnScalarCrossProduct()
    {
        double cross = new Vector2D(2.0, 3.0).Cross(new Vector2D(4.0, 5.0));

        Assert.That(cross, Is.EqualTo(-2.0));
    }

    [Test]
    public void Vector2D_AngleTo_ShouldReturnAngleInRadians()
    {
        double angle = new Vector2D(1.0, 0.0).AngleTo(new Vector2D(0.0, 1.0));

        Assert.That(angle, Is.EqualTo(Math.PI / 2.0).Within(GeometryTolerance.Default));
    }

    [Test]
    public void BoundingBox_FromPoints_ShouldCreateBounds()
    {
        BoundingBox box = BoundingBox.FromPoints(
            new Point2D(2.0, 5.0),
            new Point2D(-1.0, 3.0),
            new Point2D(4.0, -2.0));

        Assert.Multiple(() =>
        {
            Assert.That(box.MinX, Is.EqualTo(-1.0));
            Assert.That(box.MinY, Is.EqualTo(-2.0));
            Assert.That(box.MaxX, Is.EqualTo(4.0));
            Assert.That(box.MaxY, Is.EqualTo(5.0));
            Assert.That(box.Width, Is.EqualTo(5.0));
            Assert.That(box.Height, Is.EqualTo(7.0));
        });
    }

    [Test]
    public void BoundingBox_FromPointsWithoutPoints_ShouldThrow()
    {
        Assert.Throws<ArgumentException>(() => BoundingBox.FromPoints());
    }

    [Test]
    public void BoundingBox_Union_ShouldCombineBounds()
    {
        BoundingBox left = new BoundingBox(0.0, 1.0, 4.0, 5.0);
        BoundingBox right = new BoundingBox(-2.0, 2.0, 3.0, 8.0);

        BoundingBox result = left.Union(right);

        Assert.That(result, Is.EqualTo(new BoundingBox(-2.0, 1.0, 4.0, 8.0)));
    }

    [Test]
    public void BoundingBox_Contains_ShouldReturnTrueForPointInsideBounds()
    {
        BoundingBox box = new BoundingBox(0.0, 1.0, 4.0, 5.0);

        Assert.That(box.Contains(new Point2D(2.0, 3.0)), Is.True);
    }

    [Test]
    public void BoundingBox_Contains_ShouldUseTolerance()
    {
        BoundingBox box = new BoundingBox(0.0, 1.0, 4.0, 5.0);

        Assert.That(box.Contains(new Point2D(-GeometryTolerance.Default / 2.0, 3.0)), Is.True);
    }

    [Test]
    public void Matrix3x3_Identity_ShouldKeepPointUnchanged()
    {
        Point2D result = Matrix3x3.Identity.Transform(new Point2D(2.0, 3.0));

        Assert.That(result, Is.EqualTo(new Point2D(2.0, 3.0)));
    }

    [Test]
    public void Matrix3x3_CreateTranslation_ShouldTranslatePoint()
    {
        Point2D result = Matrix3x3.CreateTranslation(5.0, -2.0).Transform(new Point2D(2.0, 3.0));

        Assert.That(result, Is.EqualTo(new Point2D(7.0, 1.0)));
    }

    [Test]
    public void Matrix3x3_CreateTranslation_ShouldNotTranslateVector()
    {
        Vector2D result = Matrix3x3.CreateTranslation(5.0, -2.0).Transform(new Vector2D(2.0, 3.0));

        Assert.That(result, Is.EqualTo(new Vector2D(2.0, 3.0)));
    }

    [Test]
    public void Matrix3x3_CreateRotation_ShouldRotatePoint()
    {
        Point2D result = Matrix3x3.CreateRotation(Math.PI / 2.0).Transform(new Point2D(1.0, 0.0));

        Assert.Multiple(() =>
        {
            Assert.That(result.X, Is.EqualTo(0.0).Within(GeometryTolerance.Default));
            Assert.That(result.Y, Is.EqualTo(1.0).Within(GeometryTolerance.Default));
        });
    }

    [Test]
    public void Matrix3x3_CreateScaling_ShouldScalePoint()
    {
        Point2D result = Matrix3x3.CreateScaling(2.0, 3.0).Transform(new Point2D(4.0, 5.0));

        Assert.That(result, Is.EqualTo(new Point2D(8.0, 15.0)));
    }

    [Test]
    public void Matrix3x3_CreateReflectionX_ShouldReflectAcrossXAxis()
    {
        Point2D result = Matrix3x3.CreateReflectionX().Transform(new Point2D(4.0, 5.0));

        Assert.That(result, Is.EqualTo(new Point2D(4.0, -5.0)));
    }

    [Test]
    public void Matrix3x3_CreateReflectionY_ShouldReflectAcrossYAxis()
    {
        Point2D result = Matrix3x3.CreateReflectionY().Transform(new Point2D(4.0, 5.0));

        Assert.That(result, Is.EqualTo(new Point2D(-4.0, 5.0)));
    }

    [Test]
    public void Matrix3x3_Multiply_ShouldComposeTransforms()
    {
        Matrix3x3 transform = Matrix3x3.CreateTranslation(10.0, 0.0) * Matrix3x3.CreateScaling(2.0, 2.0);

        Point2D result = transform.Transform(new Point2D(3.0, 4.0));

        Assert.That(result, Is.EqualTo(new Point2D(16.0, 8.0)));
    }

    [Test]
    public void Matrix3x3_TransformVector_ShouldApplyLinearPart()
    {
        Matrix3x3 transform = Matrix3x3.CreateRotation(Math.PI / 2.0) * Matrix3x3.CreateScaling(2.0, 2.0);

        Vector2D result = transform.Transform(new Vector2D(1.0, 0.0));

        Assert.Multiple(() =>
        {
            Assert.That(result.X, Is.EqualTo(0.0).Within(GeometryTolerance.Default));
            Assert.That(result.Y, Is.EqualTo(2.0).Within(GeometryTolerance.Default));
        });
    }
}
