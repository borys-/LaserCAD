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

    public IReadOnlyList<ParameterId> GetRecalculationOrder(ParameterId changedParameterId)
    {
        var affectedParameters = GetAffectedSet(changedParameterId);
        var inDegrees = affectedParameters.ToDictionary(parameterId => parameterId, _ => 0);

        foreach (var parameterId in affectedParameters)
        {
            foreach (var dependencyId in GetDependencies(parameterId))
            {
                if (affectedParameters.Contains(dependencyId))
                {
                    inDegrees[parameterId]++;
                }
            }
        }

        var ready = new Queue<ParameterId>(inDegrees.Where(pair => pair.Value == 0).Select(pair => pair.Key));
        var order = new List<ParameterId>();

        while (ready.Count > 0)
        {
            var parameterId = ready.Dequeue();
            order.Add(parameterId);

            foreach (var dependentId in GetDirectDependents(parameterId).Where(affectedParameters.Contains))
            {
                inDegrees[dependentId]--;

                if (inDegrees[dependentId] == 0)
                {
                    ready.Enqueue(dependentId);
                }
            }
        }

        return order;
    }

    public bool HasCycle()
    {
        var states = new Dictionary<ParameterId, VisitState>();

        foreach (var parameterId in GetAllParameterIds())
        {
            if (VisitForCycle(parameterId, states))
            {
                return true;
            }
        }

        return false;
    }

    private HashSet<ParameterId> GetAffectedSet(ParameterId changedParameterId)
    {
        var affected = new HashSet<ParameterId>();
        var queue = new Queue<ParameterId>(GetDirectDependents(changedParameterId));

        while (queue.Count > 0)
        {
            var parameterId = queue.Dequeue();

            if (!affected.Add(parameterId))
            {
                continue;
            }

            foreach (var dependentId in GetDirectDependents(parameterId))
            {
                queue.Enqueue(dependentId);
            }
        }

        return affected;
    }

    private IEnumerable<ParameterId> GetDirectDependents(ParameterId parameterId)
    {
        return _dependencies
            .Where(pair => pair.Value.Contains(parameterId))
            .Select(pair => pair.Key);
    }

    private IEnumerable<ParameterId> GetAllParameterIds()
    {
        return _dependencies.Keys.Concat(_dependencies.Values.SelectMany(dependencies => dependencies)).Distinct();
    }

    private bool VisitForCycle(ParameterId parameterId, Dictionary<ParameterId, VisitState> states)
    {
        if (states.TryGetValue(parameterId, out var state))
        {
            return state == VisitState.Visiting;
        }

        states[parameterId] = VisitState.Visiting;

        foreach (var dependencyId in GetDependencies(parameterId))
        {
            if (VisitForCycle(dependencyId, states))
            {
                return true;
            }
        }

        states[parameterId] = VisitState.Visited;

        return false;
    }

    private enum VisitState
    {
        Visiting,
        Visited
    }
}
