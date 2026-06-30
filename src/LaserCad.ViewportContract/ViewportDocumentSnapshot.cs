namespace LaserCad.ViewportContract;

/// <summary>
/// Snapshot dokumentu CAD wysylany do viewportu jako JSON domenowy.
/// </summary>
public sealed record ViewportDocumentSnapshot(string DocumentJson);
