using System;
using System.Diagnostics;
using System.IO;

namespace LaserCad.Desktop;

/// <summary>
/// Zarzadza procesem Unity viewport uruchamianym przez desktop shell.
/// </summary>
public sealed class ViewportProcessController : IDisposable
{
    private Process? process;

    public bool IsRunning => process is { HasExited: false };

    public string ViewportExecutablePath { get; }

    public ViewportProcessController(string? viewportExecutablePath = null)
    {
        ViewportExecutablePath = viewportExecutablePath ?? ResolveDefaultViewportExecutablePath();
    }

    public bool TryStart()
    {
        if (IsRunning || !File.Exists(ViewportExecutablePath))
        {
            return false;
        }

        process = Process.Start(new ProcessStartInfo
        {
            FileName = ViewportExecutablePath,
            Arguments = "--viewport",
            UseShellExecute = true,
        });

        return process != null;
    }

    public void Dispose()
    {
        process?.Dispose();
    }

    private static string ResolveDefaultViewportExecutablePath()
    {
        var baseDirectory = AppContext.BaseDirectory;
        return Path.Combine(baseDirectory, "Viewport", "LaserCad.exe");
    }
}
