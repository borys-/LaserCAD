namespace LaserCad.Core.Parameters;

/// <summary>
/// Graf zaleznosci miedzy parametrami.
/// Uzywaj go do okreslenia, ktore parametry trzeba przeliczyc po zmianie innego parametru oraz do wykrywania cykli.
/// </summary>
public sealed class DependencyGraph
{
    private readonly Dictionary<ParameterId, HashSet<ParameterId>> _dependencies = [];

    /// <summary>
    /// Rejestruje zaleznosc parametru od innego parametru.
    /// Oznacza to, ze parameterId powinien byc przeliczony po zmianie dependsOnParameterId.
    /// </summary>
    public void AddDependency(ParameterId parameterId, ParameterId dependsOnParameterId)
    {
        if (!_dependencies.TryGetValue(parameterId, out var dependencies))
        {
            dependencies = [];
            _dependencies.Add(parameterId, dependencies);
        }

        dependencies.Add(dependsOnParameterId);
    }

    /// <summary>
    /// Zwraca bezposrednie zaleznosci wskazanego parametru.
    /// Uzywaj, aby sprawdzic, od jakich parametrow zalezy dany wynik.
    /// </summary>
    public IReadOnlyList<ParameterId> GetDependencies(ParameterId parameterId)
    {
        return _dependencies.TryGetValue(parameterId, out var dependencies)
            ? dependencies.ToArray()
            : Array.Empty<ParameterId>();
    }

    /// <summary>
    /// Zwraca kolejnosc przeliczania parametrow dotknietych zmiana.
    /// Metoda zaklada brak cykli; do bezpiecznego uzycia wybierz CalculateRecalculationOrder.
    /// </summary>
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

    /// <summary>
    /// Bezpiecznie wylicza kolejnosc przeliczania i zwraca blad, jesli graf zawiera cykl.
    /// To preferowana metoda dla normalnego workflow domenowego.
    /// </summary>
    public RecalculationOrderResult CalculateRecalculationOrder(ParameterId changedParameterId)
    {
        if (HasCycle())
        {
            return RecalculationOrderResult.Failure("Dependency graph contains a cycle.");
        }

        return RecalculationOrderResult.Success(GetRecalculationOrder(changedParameterId));
    }

    /// <summary>
    /// Zwraca wszystkie parametry zalezne bezposrednio albo posrednio od zmienionego parametru.
    /// Lista nie zawiera samego parametru zrodlowego.
    /// </summary>
    public IReadOnlyList<ParameterId> GetAffectedParameters(ParameterId changedParameterId)
    {
        return GetAffectedSet(changedParameterId).ToArray();
    }

    /// <summary>
    /// Sprawdza, czy graf zaleznosci zawiera cykl.
    /// Uzywaj przed przeliczaniem, aby uniknac nieskonczonej petli zaleznosci.
    /// </summary>
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

    /// <summary>
    /// Buduje zbior parametrow dotknietych zmiana, przechodzac po zaleznosciach odwrotnych.
    /// </summary>
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

    /// <summary>
    /// Zwraca parametry, ktore bezposrednio zaleza od wskazanego parametru.
    /// </summary>
    private IEnumerable<ParameterId> GetDirectDependents(ParameterId parameterId)
    {
        return _dependencies
            .Where(pair => pair.Value.Contains(parameterId))
            .Select(pair => pair.Key);
    }

    /// <summary>
    /// Zwraca wszystkie identyfikatory wystepujace w grafie jako parametry albo ich zaleznosci.
    /// </summary>
    private IEnumerable<ParameterId> GetAllParameterIds()
    {
        return _dependencies.Keys.Concat(_dependencies.Values.SelectMany(dependencies => dependencies)).Distinct();
    }

    /// <summary>
    /// Odwiedza graf metoda DFS i zwraca true, jesli trafi na aktualnie odwiedzany wezel.
    /// </summary>
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

    /// <summary>
    /// Stan odwiedzenia wezla podczas wykrywania cyklu.
    /// </summary>
    private enum VisitState
    {
        /// <summary>
        /// Wezel jest aktualnie na stosie rekursji.
        /// </summary>
        Visiting,

        /// <summary>
        /// Wezel i jego zaleznosci zostaly juz sprawdzone.
        /// </summary>
        Visited
    }
}
