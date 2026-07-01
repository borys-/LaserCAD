using LaserCad.Core.Documents;
using LaserCad.Export.Dxf;
using LaserCad.Export.Svg;
using NUnit.Framework;

namespace LaserCad.Tests.Release;

[TestFixture]
public sealed class ExampleProjectRegressionTests
{
    [Test]
    public void ExampleProjects_ShouldLoadRoundTripAndExport()
    {
        var projectFiles = Directory.GetFiles(FindExamplesDirectory(), "*.lasercad.json");

        Assert.That(projectFiles, Is.Not.Empty);

        var serializer = new DocumentSerializer();
        var svgExporter = new SvgExporter();
        var dxfExporter = new DxfExporter();

        foreach (var projectFile in projectFiles)
        {
            var document = serializer.Deserialize(File.ReadAllText(projectFile));
            var roundTripped = serializer.Deserialize(serializer.Serialize(document));
            var svg = svgExporter.Export(roundTripped, new SvgExportOptions());
            var dxf = dxfExporter.Export(roundTripped, new DxfExportOptions());

            Assert.That(roundTripped.Sketches, Is.Not.Empty, projectFile);
            Assert.That(svg, Does.Contain("<svg"), projectFile);
            Assert.That(svg, Does.Contain("Laser CAD"), projectFile);
            Assert.That(dxf, Does.Contain("SECTION"), projectFile);
        }
    }

    private static string FindExamplesDirectory()
    {
        var directory = TestContext.CurrentContext.TestDirectory;

        while (directory is not null)
        {
            var candidate = Path.Combine(directory, "examples", "projects");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = Directory.GetParent(directory)?.FullName;
        }

        throw new DirectoryNotFoundException("Nie znaleziono katalogu examples/projects.");
    }
}
