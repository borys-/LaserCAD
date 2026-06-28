namespace LaserCad.Core.Parameters;

public sealed class DependencyGraph
{
    private readonly Dictionary<ParameterId, HashSet<ParameterId>> _dependencies = [];

    public void AddDependency(ParameterId parameterId, ParameterId dependsOnParameterId)
    {
        if (!_dependencies.TryGetValue(parameterId, out var dependencies))
        {
            dependencies = [];
            _dependencies.Add(parameterId, dependencies);
        }

        dependencies.Add(dependsOnParameterId);
    }

    public IReadOnlyList<ParameterId> GetDependencies(ParameterId parameterId)
    {
        return _dependencies.TryGetValue(parameterId, out var dependencies)
            ? dependencies.ToArray()
            : Array.Empty<ParameterId>();
    }
}
