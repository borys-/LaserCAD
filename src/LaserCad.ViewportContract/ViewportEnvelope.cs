using System.Text.Json;

namespace LaserCad.ViewportContract;

/// <summary>
/// Koperta komunikatu IPC serializowana jako pojedynczy JSON.
/// </summary>
public sealed record ViewportEnvelope(ViewportMessageKind Kind, JsonElement Payload);
