using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace LaserCad.Desktop;

/// <summary>
/// Zarzadza procesem Unity viewport uruchamianym przez desktop shell.
/// </summary>
public sealed class ViewportProcessController : IDisposable
{
    private const int GwlStyle = -16;
    private const int WsChild = 0x40000000;
    private const int WsCaption = 0x00C00000;
    private const int WsThickFrame = 0x00040000;
    private const int WsMinimizeBox = 0x00020000;
    private const int WsMaximizeBox = 0x00010000;
    private const int WsSysMenu = 0x00080000;
    private const int WmMouseWheel = 0x020A;

    private Process? process;
    private IntPtr parentWindowHandle;
    private IntPtr viewportWindowHandle;

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
            Arguments = "--viewport -popupwindow",
            UseShellExecute = true,
        });

        return process != null;
    }

    public bool TryStartEmbedded(IntPtr hostWindowHandle)
    {
        parentWindowHandle = hostWindowHandle;
        if (!TryStart())
        {
            return false;
        }

        viewportWindowHandle = WaitForViewportWindowHandle(process!, TimeSpan.FromSeconds(10));
        if (viewportWindowHandle == IntPtr.Zero)
        {
            Stop();
            return false;
        }

        EmbedViewportWindow();
        return true;
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
        viewportWindowHandle = IntPtr.Zero;
        parentWindowHandle = IntPtr.Zero;
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

    private void EmbedViewportWindow()
    {
        SetParent(viewportWindowHandle, parentWindowHandle);

        var style = GetWindowLong(viewportWindowHandle, GwlStyle);
        style &= ~(WsCaption | WsThickFrame | WsMinimizeBox | WsMaximizeBox | WsSysMenu);
        style |= WsChild;
        SetWindowLong(viewportWindowHandle, GwlStyle, style);

        ResizeEmbeddedViewport();
    }

    public void ResizeEmbeddedViewport()
    {
        if (viewportWindowHandle == IntPtr.Zero || parentWindowHandle == IntPtr.Zero)
        {
            return;
        }

        if (!GetClientRect(parentWindowHandle, out var rect))
        {
            return;
        }

        MoveWindow(
            viewportWindowHandle,
            0,
            0,
            rect.Right - rect.Left,
            rect.Bottom - rect.Top,
            true);
    }

    public void FocusViewport()
    {
        if (viewportWindowHandle == IntPtr.Zero)
        {
            return;
        }

        SetFocus(viewportWindowHandle);
        SetActiveWindow(viewportWindowHandle);
    }

    public bool ForwardMouseWheel(IntPtr wParam, IntPtr lParam)
    {
        if (viewportWindowHandle == IntPtr.Zero)
        {
            return false;
        }

        FocusViewport();
        SendMessage(viewportWindowHandle, WmMouseWheel, wParam, lParam);
        return true;
    }

    private static IntPtr WaitForViewportWindowHandle(Process viewportProcess, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;

        while (DateTime.UtcNow < deadline)
        {
            viewportProcess.Refresh();
            if (viewportProcess.MainWindowHandle != IntPtr.Zero)
            {
                return viewportProcess.MainWindowHandle;
            }

            Thread.Sleep(100);
        }

        return IntPtr.Zero;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetParent(IntPtr childWindowHandle, IntPtr newParentWindowHandle);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr windowHandle, int index);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr windowHandle, int index, int newLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool MoveWindow(IntPtr windowHandle, int x, int y, int width, int height, bool repaint);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetFocus(IntPtr windowHandle);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetActiveWindow(IntPtr windowHandle);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SendMessage(IntPtr windowHandle, int message, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetClientRect(IntPtr windowHandle, out NativeRect rect);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;

        public int Top;

        public int Right;

        public int Bottom;
    }
}
