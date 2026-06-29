using System;
using System.IO;
using UnityEditor;

namespace LaserCad.Unity.Editor
{
    /// <summary>
    /// Buduje desktopowa aplikacje Unity z linii komend.
    /// </summary>
    public static class BuildPlayer
    {
        private const string WorkspaceScenePath = "Assets/Scenes/Workspace.unity";
        private const string DefaultOutputPath = "../bin/release/LaserCad/LaserCad.exe";

        /// <summary>
        /// Buduje aplikacje Windows x64 do katalogu podanego przez argument `-buildOutput`.
        /// </summary>
        public static void BuildWindows()
        {
            var outputPath = GetArgumentValue("-buildOutput") ?? DefaultOutputPath;
            var normalizedOutputPath = Path.GetFullPath(outputPath);
            var outputDirectory = Path.GetDirectoryName(normalizedOutputPath);

            if (string.IsNullOrEmpty(outputDirectory))
            {
                throw new InvalidOperationException("Nie mozna ustalic katalogu wyjsciowego builda Unity.");
            }

            Directory.CreateDirectory(outputDirectory);

            var report = BuildPipeline.BuildPlayer(
                new[] { WorkspaceScenePath },
                normalizedOutputPath,
                BuildTarget.StandaloneWindows64,
                BuildOptions.None);

            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                throw new InvalidOperationException("Build Unity zakonczyl sie statusem: " + report.summary.result);
            }
        }

        private static string GetArgumentValue(string argumentName)
        {
            var args = Environment.GetCommandLineArgs();

            for (var i = 0; i < args.Length - 1; i++)
            {
                if (string.Equals(args[i], argumentName, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }

            return null;
        }
    }
}
