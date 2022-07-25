using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooruApi;
using OsuApiHelper;

namespace Test
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var osuApi = new OsuApi(new OsuApiConfig()
            {
                ClientId = "14302",
                Secret = "KmFIn67KoUzuLhL7PLRCsZGxKeR50JEjxcYWxQ28"
            });

            var result = await osuApi.BeatmapsetDownload("405011");

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

        var booru = new GelbooruApi();

            //var autocomplete = await booru.GetAutoComplete("vermei");

            //var posts = await booru.GetPosts("vermeil_(arknights)");

            var requestHelper = new GelbooruPostQueryHelper()
            {
                Tags = new List<Tag>()
                {
                    new Tag()
                    {
                       Value = "mostima_(arknights)",
                    }
                },
                Limit = 1,
                Page = 0,
                Score = new Score()
                {
                    CompareType = CompareType.EqualGreater,
                    Value = 75,
                },
                Sort = new Sort()
                {
                    Type = SortType.Random,
                    RandomSeed = 300,
                },
                Size = new Size()
                {
                    Width = new RangeValue()
                    {
                        CompareType = CompareType.EqualGreater,
                        Value = 1920,
                    },
                    Height = new RangeValue()
                    {
                        CompareType = CompareType.EqualGreater,
                        Value = 1080,
                    }
                },
                Rating = new List<RatingGelbooru>()
                {
                    new RatingGelbooru()
                    {
                        PostRating = GelbooruPostRating.Explicit,
                        SearchType = SearchType.Exclude,
                    }
                },
            };

            var query = requestHelper.ToString();

            var posts1 = booru.GetPostsUrl(requestHelper);

            Console.WriteLine(posts1);

            //var posts2 = await booru.GetPostsAsync(requestHelper);
        }
    }
}
