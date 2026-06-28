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
}
