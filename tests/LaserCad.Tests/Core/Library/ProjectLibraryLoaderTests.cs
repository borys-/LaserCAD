using LaserCad.Core.Library;

namespace LaserCad.Tests.Core.Library;

public sealed class ProjectLibraryLoaderTests
{
    [Test]
    public void Load_WithRepositoryLibrary_ShouldLoadMaterialsAndTemplates()
    {
        var loader = new ProjectLibraryLoader();
        var library = loader.Load(Path.Combine(FindRepositoryRoot(), "library"));

        Assert.That(library.Materials, Has.Count.EqualTo(2));
        Assert.That(library.Materials.Select(material => material.Id), Does.Contain("plywood-3mm"));
        Assert.That(library.Materials.Select(material => material.Profile.Name), Does.Contain("Sklejka 4 mm"));
        Assert.That(library.Materials.Single(material => material.Id == "plywood-3mm").Profile.Thickness.Millimeters, Is.EqualTo(3.0));

        Assert.That(library.Templates, Has.Count.EqualTo(4));
        Assert.That(library.Templates.Select(template => template.GeneratorType), Does.Contain("Box"));
        Assert.That(library.Templates.Select(template => template.GeneratorType), Does.Contain("Organizer"));
        Assert.That(library.Templates.Count(template => template.GeneratorType == "Stand"), Is.EqualTo(2));
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(TestContext.CurrentContext.WorkDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "TASKS.md")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Nie znaleziono katalogu repozytorium.");
    }
}
