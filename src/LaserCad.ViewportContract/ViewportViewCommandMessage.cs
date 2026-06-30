namespace LaserCad.ViewportContract;

/// <summary>
/// Komunikat intencji zmiany widoku viewportu.
/// </summary>
public sealed record ViewportViewCommandMessage(ViewportViewCommand Command, bool? Enabled = null);
