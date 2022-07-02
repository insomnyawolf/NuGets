using System.Threading.Tasks;
using System;
using Extensions;
using SaucenaoSearch;
using BooruApi;

namespace Test
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            //var saucenao = new SaucenaoWebInterface();

            //var data = await saucenao.GetSauceAsync("https://media.discordapp.net/attachments/639815892565229579/928656817511342090/4574.jpg?width=1141&height=671");

            //foreach (var item in data)
            //{
            //    if (!string.IsNullOrEmpty(item.SourceUrl))
            //    {
            //        var uri = new Uri(item.SourceUrl);
            //        uri.OpenInBrowser();
            //        return;
            //    }
            //}

            var booru = new BooruApiRequester(BooruServer.Gelbooru);

            var autocomplete = await booru.GetAutoComplete("vermei");

            var posts = await booru.GetPosts("vermeil_(arknights)");

            var requestHelper = new GelbooruPostQueryHelper()
            {
                Tags = new System.Collections.Generic.List<string>()
                {
                    "vermeil_(arknights)"
                },
                Limit = 1,
                Page = 0,
                Random = true,
                Rating = BooruPostRating.Safe,
            };

            var posts1 = await booru.GetPosts(requestHelper);

            var posts2 = await booru.GetPosts(requestHelper);

            var posts3 = await booru.GetPosts(requestHelper);
        }
    }
}
