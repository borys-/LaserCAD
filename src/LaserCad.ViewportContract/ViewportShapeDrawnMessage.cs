namespace LaserCad.ViewportContract;

/// <summary>
/// Zdarzenie narysowania podstawowego ksztaltu w viewportcie.
/// </summary>
public sealed record ViewportShapeDrawnMessage(ViewportDrawingTool Tool, ViewportPoint Start, ViewportPoint End);
