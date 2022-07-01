using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Extensions
{
    public static class Os
    {
        public static void OpenInBrowser(this Uri Uri)
        {
            var url = Uri.ToString();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                Process.Start(url);
            }
        }

        public static void ForceKillAsync(this Process process)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                Process.Start(new ProcessStartInfo("Taskkill", $"/T /F /PID {process.Id}") { UseShellExecute = false });
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                Process.Start(new ProcessStartInfo("kill", $"-s SIGKILL {process.Id}") { UseShellExecute = false });
            }
            else
            {
                // Unsupported
                throw new Exception("Unsupportet platform (yet)");
            }
        }
    }
}
