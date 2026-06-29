using LaserCad.Geometry;

namespace LaserCad.Core.Documents;

/// <summary>
/// Encja luku okregu w szkicu.
/// </summary>
public sealed class ArcEntity : Entity
{
    private const double FullTurnRadians = Math.PI * 2.0;

    /// <summary>
    /// Tworzy encje luku na podstawie geometrii 2D.
    /// </summary>
    public ArcEntity(Arc2D arc, Guid? id = null, string layerName = "Cut")
        : base(id, layerName)
    {
        Arc = arc;
    }

    /// <summary>
    /// Geometria luku.
    /// </summary>
    public Arc2D Arc { get; }

    /// <inheritdoc />
    public override BoundingBox Bounds => BoundingBox.FromPoints(GetBoundsPoints().ToArray());

    /// <inheritdoc />
    public override ISketchEntity Transform(Matrix3x3 transform)
    {
        return new ArcEntity(Arc.Transform(transform), Id, LayerName);
    }

    /// <inheritdoc />
    public override Entity Copy(Guid? id = null)
    {
        return new ArcEntity(Arc, id, LayerName);
    }

    private IEnumerable<Point2D> GetBoundsPoints()
    {
        yield return Arc.PointAt(0.0);
        yield return Arc.PointAt(1.0);

        double[] cardinalAngles =
        {
            0.0,
            Math.PI / 2.0,
            Math.PI,
            Math.PI * 1.5,
        };

        foreach (double angle in cardinalAngles)
        {
            if (ContainsAngle(angle))
            {
                yield return new Point2D(
                    Arc.Center.X + (Math.Cos(angle) * Arc.Radius),
                    Arc.Center.Y + (Math.Sin(angle) * Arc.Radius));
            }
        }
    }

    private bool ContainsAngle(double angle)
    {
        double sweep = GetSweepAngle();
        double relative = Arc.Direction == ArcDirection.Counterclockwise
            ? NormalizePositiveAngle(angle - Arc.StartAngleRadians)
            : NormalizePositiveAngle(Arc.StartAngleRadians - angle);

        return relative <= sweep + GeometryTolerance.Default;
    }

    private double GetSweepAngle()
    {
        double delta = Arc.Direction == ArcDirection.Counterclockwise
            ? Arc.EndAngleRadians - Arc.StartAngleRadians
            : Arc.StartAngleRadians - Arc.EndAngleRadians;

        return NormalizePositiveAngle(delta);
    }

    private static double NormalizePositiveAngle(double angle)
    {
        double result = angle % FullTurnRadians;

        if (result < 0.0)
        {
            result += FullTurnRadians;
        }

        return result;
    }
}
