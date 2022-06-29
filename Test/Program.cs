using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SaucenaoApi;

namespace Test
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var saucenao = new Saucenao();

            var data = await saucenao.GetSauceAsync("https://media.discordapp.net/attachments/639815892565229579/928656817511342090/4574.jpg?width=1141&height=671");

            foreach (var item in data)
            {
                if (!string.IsNullOrEmpty(item.SourceUrl))
                {
                    openInBrowser(item.SourceUrl);
                    return;
                }
            }
        }

        private static void openInBrowser(string url)
        {
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
    }
}
