namespace LaserCad.Core.Production;

/// <summary>
/// Wynik prostego nestingu elementow na arkuszu.
/// </summary>
public sealed class NestingResult
{
    private readonly NestedPart[] parts;
    private readonly IReadOnlyList<NestedPart> readOnlyParts;

    /// <summary>
    /// Tworzy wynik nestingu.
    /// </summary>
    public NestingResult(IEnumerable<NestedPart> parts)
    {
        if (parts is null)
        {
            throw new ArgumentNullException(nameof(parts));
        }

        this.parts = parts.ToArray();
        readOnlyParts = Array.AsReadOnly(this.parts);
    }

    /// <summary>
    /// Elementy ulozone na arkuszu.
    /// </summary>
    public IReadOnlyList<NestedPart> Parts => readOnlyParts;
}
