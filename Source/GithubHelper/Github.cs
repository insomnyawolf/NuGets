using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;
using GithubHelper.Models;

namespace GithubHelper
{
    public static class GithubTools
    {
        private static readonly HttpClient HttpClient;

        static GithubTools()
        {
            HttpClient = new();
            HttpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("(+inso/GithubTools/testver)"));
        }

        private static string FixUrl(string url)
        {
            if (!url.EndsWith('/'))
            {
                url += '/';
            }
            return url;
        }

        public static void ExceptionToIssueTemplate(string url, Action program)
        {
            try
            {
                program();
            }
            catch (Exception ex)
            {
                ExceptionToIssueTemplate(url, ex);
            }
        }

        public static void ExceptionToIssueTemplate(string url, Exception exception)
        {
            url = FixUrl(url);

            var stack = exception.ToString();

            // Prepare URL.
            const string issueTitle = "UnhandledException";
            string issueBody = WebUtility.UrlEncode($"StackTrace\n```\n{stack}\n```");

            TryOpenUrlInBrowser($"{url}issues/new?title={issueTitle}&body={issueBody}");
        }

        private static bool TryOpenUrlInBrowser(string url)
        {
            // Navigate to a URL.
            if (OperatingSystem.IsWindows())
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}")
                {
                    CreateNoWindow = true,
                });
                return true;
            }

            if (OperatingSystem.IsLinux())
            {
                Process.Start("xdg-open", url);
                return true;
            }

            if (OperatingSystem.IsMacOS())
            {
                Process.Start("open", url);
                return true;
            }

#if RELEASE
            return false;
#else
            throw new NotImplementedException($"'{nameof(TryOpenUrlInBrowser)}' is not implemented for '{RuntimeInformation.OSDescription}'");
#endif

        }

        private static string GetApiUrlFromNormalUrl(string url)
        {
            // https://github.com/frida/frida
            return url.Replace("https://github.com/", "https://api.github.com/repos/");
            // https://api.github.com/repos/frida/frida
        }

        public static async Task<Release?> GetLastestReleaseAvailable(string url)
        {
            var apiurl = GetApiUrlFromNormalUrl(url);

            // https://api.github.com/repos/frida/frida
            apiurl += "/releases/latest";
            // https://api.github.com/repos/frida/frida/releases/latest

            var response = await HttpClient.GetAsync(apiurl);

            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStreamAsync();
            var release = await JsonSerializer.DeserializeAsync<Release>(responseContent);

            return release;
        }

        public static ReleaseAsset GetAssetOrDefault(this Release release, string assetName)
        {
            return release.Assets.FirstOrDefault(item => item.Name == assetName);
        }
    }
}