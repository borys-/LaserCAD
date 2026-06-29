using LaserCad.Core.Commands;
using LaserCad.Core.Documents;
using LaserCad.Core.FeatureTree;
using LaserCad.Geometry;

namespace LaserCad.Tests.Core.FeatureTree;

public sealed class FeatureTreeRebuildTests
{
    [Test]
    public void Rebuild_WithGeneratorAndMove_ShouldReturnMovedGeneratedGeometry()
    {
        var line = new LineEntity(new LineSegment2D(new Point2D(1.0, 1.0), new Point2D(3.0, 1.0)));
        var sketch = new Sketch(name: "Generated sketch", entities: new[] { line });
        var generator = new GeneratorInstance(generatorType: "TestGenerator");
        var generatorFeature = new GeneratorFeatureTreeItem(generator, new[] { sketch });
        var moveFeature = new EditOperationFeatureTreeItem(
            new MoveCommand(sketch.Id, line.Id, 5.0, 0.0),
            name: "Move generated line");
        var tree = new LaserCad.Core.FeatureTree.FeatureTree(new FeatureTreeItem[]
        {
            generatorFeature,
            moveFeature,
        });

        var rebuilt = tree.Rebuild(new CadDocument());

        var moved = (LineEntity)rebuilt.Sketches[0].Entities[0];
        Assert.That(rebuilt.Generators, Has.Count.EqualTo(1));
        Assert.That(rebuilt.Generators[0], Is.SameAs(generator));
        Assert.That(moved.Segment.Start.X, Is.EqualTo(6.0).Within(GeometryTolerance.Default));
        Assert.That(moved.Segment.End.X, Is.EqualTo(8.0).Within(GeometryTolerance.Default));
    }
}
