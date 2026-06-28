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
}
