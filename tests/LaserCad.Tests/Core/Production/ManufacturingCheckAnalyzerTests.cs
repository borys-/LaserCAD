using LaserCad.Core.Documents;
using LaserCad.Core.Production;
using LaserCad.Geometry;
using NUnit.Framework;

namespace LaserCad.Tests.Core.Production;

[TestFixture]
public class ManufacturingCheckAnalyzerTests
{
    [Test]
    public void Analyze_WithDuplicateLine_ShouldReturnDuplicateLineError()
    {
        var line = new LineEntity(new LineSegment2D(new Point2D(0.0, 0.0), new Point2D(10.0, 0.0)));
        var duplicate = new LineEntity(new LineSegment2D(new Point2D(10.0, 0.0), new Point2D(0.0, 0.0)));
        var document = CreateDocument(line, duplicate);

        var checks = new ManufacturingCheckAnalyzer().Analyze(document);

        Assert.That(checks, Has.Some.Matches<ManufacturingCheck>(check =>
            check.Code == "DuplicateLine" &&
            check.Severity == ManufacturingCheckSeverity.Error &&
            check.EntityId == duplicate.Id));
    }

    private static CadDocument CreateDocument(params Entity[] entities)
    {
        return new CadDocument(sketches: new[] { new Sketch(entities: entities) });
    }
}
