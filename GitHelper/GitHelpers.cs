using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using GitHelper.Models;
using Microsoft.Extensions.Logging;

namespace GitHelper
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Branch
    {
        Release,
        Prerelease,
    }

    public class GitHelpersConfig
    {
        public string GitUrl { get; set; }
        public bool AutoUpdate { get; set; } = true;
        public Branch Branch { get; set; } = Branch.Release;

        [JsonIgnore]
        public ILogger? Logger { get; set; }
    }

    public class GitHelpers
    {
        private static readonly HttpClient HttpClient = new();

        private GitHelpersConfig Config { get; set; }

        public GitHelpers(GitHelpersConfig config)
        {
            if (config == null || string.IsNullOrWhiteSpace(config.GitUrl))
            {
                throw new InvalidDataException($"You must provide a valid {nameof(GitHelpersConfig)} config with a valid git url");
            }

            Config = config;

            if (!Config.GitUrl.EndsWith('/'))
            {
                Config.GitUrl += '/';
            }
        }


        public void Run(Action program)
        {
            try
            {
                program();
            }
            catch (Exception ex)
            {
                var exception = ex.ToString();

                Console.WriteLine(exception);
                Config.Logger?.LogCritical(exception);

#if RELEASE
                OpenGitReport(exception);
#endif
            }
        }

        private void OpenGitReport(string stack)
        {
            // Prepare URL.
            const string issueTitle = "UnhandledCrash";
            string issueBody = WebUtility.UrlEncode($"StackTrace\n```\n{stack}\n```");

            OpenUrlInBrowser($"{Config.GitUrl}issues/new?title={issueTitle}&body={issueBody}");
        }

        private void OpenUrlInBrowser(string url)
        {
            // Navigate to a URL.
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
                {
                    CreateNoWindow = true,
                });
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
                throw new NotImplementedException($"'{nameof(OpenUrlInBrowser)}' is not implemented for '{RuntimeInformation.OSDescription}'");
            }
        }

        public static async Task<Release?> CheckLastestAvailableRelease(string url, Branch branch)
        {
            var Response = await HttpClient.GetStringAsync(url);
            var versiones = JsonSerializer.Deserialize<List<Release>>(Response);

            if (versiones == null || versiones.Count < 1)
            {
                return null;
            }

            Release lastest;

            // Any will do
            if (branch == Branch.Prerelease)
            {
                return versiones[0];
            }

            // filter to only stable versions
            for (int index = 0; index < versiones.Count; index++)
            {
                var c = versiones[index];
                if (!c.Prerelease)
                {
                    return c;
                }
            }

            // no version found
            return null;
        }

        public static async Task<string?> GetDownloadUrl(Release release)
        {
            var assets = release.Assets;
            if (assets == null || assets.Count < 1)
            {
                return null;
            }

#error check platform specific (?)
        }
    }
}