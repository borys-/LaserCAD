using System;

namespace LaserCad.Unity
{
    /// <summary>
    /// Rozpoznaje, czy Unity player zostal uruchomiony jako osobny proces viewportu.
    /// </summary>
    public static class ViewportProcessMode
    {
        public static bool IsViewportProcess()
        {
            var args = Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], "--viewport", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
