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

    public bool Stop()
    {
        if (!IsRunning || process == null)
        {
            return false;
        }

        process.CloseMainWindow();
        if (!process.WaitForExit(3000))
        {
            process.Kill();
        }

        process.Dispose();
        process = null;
        return true;
    }

    public bool Restart()
    {
        Stop();
        return TryStart();
    }

    public void Dispose()
    {
        Stop();
        process?.Dispose();
    }

    private static string ResolveDefaultViewportExecutablePath()
    {
        var baseDirectory = AppContext.BaseDirectory;
        return Path.Combine(baseDirectory, "Viewport", "LaserCad.exe");
    }
}
