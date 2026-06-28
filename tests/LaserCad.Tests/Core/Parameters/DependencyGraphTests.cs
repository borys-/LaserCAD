using LaserCad.Core.Parameters;

namespace LaserCad.Tests.Core.Parameters;

public sealed class DependencyGraphTests
{
    [Test]
    public void Constructor_ShouldCreateDependencyGraph()
    {
        var graph = new DependencyGraph();

        Assert.That(graph, Is.Not.Null);
    }

    [Test]
    public void AddDependency_ShouldRegisterDependency()
    {
        var graph = new DependencyGraph();
        var width = new ParameterId("Width");
        var materialThickness = new ParameterId("MaterialThickness");

        graph.AddDependency(width, materialThickness);

        Assert.That(graph.GetDependencies(width), Is.EqualTo(new[] { materialThickness }));
    }

    [Test]
    public void GetDependencies_WithoutRegisteredDependencies_ShouldReturnEmptyList()
    {
        var graph = new DependencyGraph();

        var dependencies = graph.GetDependencies(new ParameterId("Width"));

        Assert.That(dependencies, Is.Empty);
    }

    [Test]
    public void GetRecalculationOrder_ForChangedDependency_ShouldReturnDependentParameter()
    {
        var graph = new DependencyGraph();
        var width = new ParameterId("Width");
        var innerWidth = new ParameterId("InnerWidth");

        graph.AddDependency(innerWidth, width);

        var order = graph.GetRecalculationOrder(width);

        Assert.That(order, Is.EqualTo(new[] { innerWidth }));
    }

    [Test]
    public void HasCycle_ForAcyclicGraph_ShouldReturnFalse()
    {
        var graph = new DependencyGraph();

        graph.AddDependency(new ParameterId("InnerWidth"), new ParameterId("Width"));

        Assert.That(graph.HasCycle(), Is.False);
    }

    [Test]
    public void HasCycle_ForCyclicGraph_ShouldReturnTrue()
    {
        var graph = new DependencyGraph();
        var width = new ParameterId("Width");
        var innerWidth = new ParameterId("InnerWidth");

        graph.AddDependency(innerWidth, width);
        graph.AddDependency(width, innerWidth);

        Assert.That(graph.HasCycle(), Is.True);
    }

    [Test]
    public void CalculateRecalculationOrder_ForCycle_ShouldReturnError()
    {
        var graph = new DependencyGraph();
        var width = new ParameterId("Width");
        var innerWidth = new ParameterId("InnerWidth");

        graph.AddDependency(innerWidth, width);
        graph.AddDependency(width, innerWidth);

        var result = graph.CalculateRecalculationOrder(width);

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Order, Is.Empty);
        Assert.That(result.Error, Is.EqualTo("Dependency graph contains a cycle."));
    }

    [Test]
    public void GetAffectedParameters_ShouldReturnDirectAndTransitiveDependents()
    {
        var graph = new DependencyGraph();
        var width = new ParameterId("Width");
        var innerWidth = new ParameterId("InnerWidth");
        var area = new ParameterId("Area");

        graph.AddDependency(innerWidth, width);
        graph.AddDependency(area, innerWidth);

        var affectedParameters = graph.GetAffectedParameters(width);

        Assert.That(affectedParameters, Is.EquivalentTo(new[] { innerWidth, area }));
    }
}
