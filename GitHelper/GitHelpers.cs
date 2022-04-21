using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
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
    }
}