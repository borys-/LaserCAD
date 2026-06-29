using LaserCad.Geometry;
using LaserCad.Geometry.Units;

namespace LaserCad.Core.FingerJoints;

/// <summary>
/// Wynik wygenerowania profilu polaczenia palcowego dla jednej prostej krawedzi.
/// </summary>
public sealed class FingerJointProfile
{
    /// <summary>
    /// Tworzy wynik generatora profilu polaczenia palcowego.
    /// </summary>
    public FingerJointProfile(
        IReadOnlyList<FingerJointSegment> segments,
        IReadOnlyList<Point2D> points,
        Length materialThickness,
        Length outwardDepth,
        Length slotInset,
        Length kerfCompensation,
        Length clearanceCompensation)
    {
        if (segments is null)
        {
            throw new ArgumentNullException(nameof(segments));
        }

        if (points is null)
        {
            throw new ArgumentNullException(nameof(points));
        }

        Segments = segments.ToArray();
        Points = points.ToArray();
        MaterialThickness = materialThickness;
        OutwardDepth = outwardDepth;
        SlotInset = slotInset;
        KerfCompensation = kerfCompensation;
        ClearanceCompensation = clearanceCompensation;
    }

    /// <summary>
    /// Segmenty podzialu krawedzi w kolejnosci od poczatku do konca.
    /// </summary>
    public IReadOnlyList<FingerJointSegment> Segments { get; }

    /// <summary>
    /// Punkty lamanej profilu gotowej do uzycia jako krawedz konturu.
    /// </summary>
    public IReadOnlyList<Point2D> Points { get; }

    /// <summary>
    /// Grubosc materialu uzyta do wyznaczenia glebokosci palcow.
    /// </summary>
    public Length MaterialThickness { get; }

    /// <summary>
    /// Wysuniecie palca na zewnatrz bazowej krawedzi.
    /// </summary>
    public Length OutwardDepth { get; }

    /// <summary>
    /// Dodatkowe cofniecie wciecia wynikajace z luzu i kerfu.
    /// </summary>
    public Length SlotInset { get; }

    /// <summary>
    /// Kompensacja szczeliny ciecia zastosowana w profilu.
    /// </summary>
    public Length KerfCompensation { get; }

    /// <summary>
    /// Kompensacja luzu montazowego zastosowana w profilu.
    /// </summary>
    public Length ClearanceCompensation { get; }
}
