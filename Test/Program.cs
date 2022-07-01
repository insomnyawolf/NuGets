using System.Threading.Tasks;
using System;
using Extensions;
using SaucenaoSearch;

namespace Test
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var saucenao = new SaucenaoWebInterface();

            var data = await saucenao.GetSauceAsync("https://media.discordapp.net/attachments/639815892565229579/928656817511342090/4574.jpg?width=1141&height=671");

            foreach (var item in data)
            {
                if (!string.IsNullOrEmpty(item.SourceUrl))
                {
                    var uri = new Uri(item.SourceUrl);
                    uri.OpenInBrowser();
                    return;
                }
            }
        }

        
    }
}
