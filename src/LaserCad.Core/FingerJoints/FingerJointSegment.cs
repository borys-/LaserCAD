using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.FingerJoints;

/// <summary>
/// Opis pojedynczego segmentu podzialu krawedzi polaczenia palcowego.
/// </summary>
public sealed class FingerJointSegment
{
    /// <summary>
    /// Tworzy segment profilu polaczenia palcowego.
    /// </summary>
    public FingerJointSegment(FingerJointSegmentKind kind, Length startOffset, Length length, Point2D start, Point2D end)
    {
        if (length <= Length.FromMillimeters(0.0))
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Segment length must be greater than zero.");
        }

        Kind = kind;
        StartOffset = startOffset;
        Length = length;
        Start = start;
        End = end;
    }

    /// <summary>
    /// Typ segmentu: palec albo wciecie.
    /// </summary>
    public FingerJointSegmentKind Kind { get; }

    /// <summary>
    /// Odleglosc poczatku segmentu od poczatku bazowej krawedzi.
    /// </summary>
    public Length StartOffset { get; }

    /// <summary>
    /// Dlugosc segmentu mierzona wzdluz bazowej krawedzi.
    /// </summary>
    public Length Length { get; }

    /// <summary>
    /// Punkt poczatkowy segmentu na bazowej krawedzi.
    /// </summary>
    public Point2D Start { get; }

    /// <summary>
    /// Punkt koncowy segmentu na bazowej krawedzi.
    /// </summary>
    public Point2D End { get; }
}
