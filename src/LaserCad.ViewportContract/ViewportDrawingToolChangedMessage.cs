namespace LaserCad.ViewportContract;

/// <summary>
/// Komunikat zmiany aktywnego narzedzia rysowania w viewportcie.
/// </summary>
public sealed record ViewportDrawingToolChangedMessage(ViewportDrawingTool Tool);
