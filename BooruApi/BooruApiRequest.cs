using BooruApi.Models;
using System.Text.Json;

namespace BooruApi
{
    // https://github.com/insomnyawolf/Gonnachan/blob/master/const.go
    public class BooruApiRequester
    {
        private HttpClient HttpClient { get; set; }
        private string BaseAddress { get; set; }
        private BooruServer ServerType { get; set; }

        private static readonly Dictionary<BooruServer, string> ServerAdresses = new()
        {
            { BooruServer.Gelbooru, "https://gelbooru.com/index.php" },
            { BooruServer.Safebooru, "https://safebooru.org/index.php?page=dapi&q=index&json=1&s=post&" },
            { BooruServer.Rule34, "https://rule34.xxx/index.php?page=dapi&q=index&json=1&s=post&" },
            { BooruServer.TheBigImageboard, "https://tbib.org/index.php?page=dapi&q=index&json=1&s=post&" },
            { BooruServer.Konachan, "https://konachan.com/post.json?" },
            { BooruServer.Yandere, "https://yande.re/post.json?" },
        };

        private static readonly Dictionary<BooruServer, BooruServer> ServerTypes = new()
        {
            { BooruServer.Gelbooru, BooruServer.Gelbooru },
            { BooruServer.Safebooru, BooruServer.Gelbooru },
            { BooruServer.Rule34, BooruServer.Gelbooru },
            { BooruServer.TheBigImageboard, BooruServer.Gelbooru },
            { BooruServer.Konachan, BooruServer.Konachan },
            { BooruServer.Yandere, BooruServer.Konachan },
        };

        private static readonly Dictionary<BooruServer, Dictionary<BooruApiEndpoint, string>> Endpoints = new();

        static BooruApiRequester()
        {
            Endpoints.Add(BooruServer.Gelbooru, new Dictionary<BooruApiEndpoint, string>
            {
                { BooruApiEndpoint.PostsApi, "?page=dapi&q=index&json=1&s=post&tags=" },
                { BooruApiEndpoint.PostsPage, "?page=post&s=view&id=" },
                { BooruApiEndpoint.AutoComplete, "?page=autocomplete2&type=tag_query&term=" }
            });
        }



        public BooruApiRequester(BooruServer server, HttpClient HttpClient = null) : this(ServerAdresses[server], ServerTypes[server], HttpClient)
        {

        }

        public BooruApiRequester(string BaseAddress, BooruServer ServerType, HttpClient HttpClient = null)
        {
            if (this.HttpClient == null)
            {
                this.HttpClient = new HttpClient();
            }
            else
            {
                this.HttpClient = HttpClient;
            }

            this.BaseAddress = BaseAddress;
            this.ServerType = ServerType;
        }

//#warning GetStringAsync to GetStreamAsync when working

        public async Task<List<AutoCompleteResponse>> GetAutoComplete(string input)
        {
            var requestAddress = GetEndpointUrl(BooruApiEndpoint.AutoComplete) + input;

            var stream = await this.HttpClient.GetStreamAsync(requestAddress);

            return JsonSerializer.Deserialize<List<AutoCompleteResponse>>(stream);
        }

        public async Task<ApiResponse<Post>> GetPosts(string tags)
        {
            var requestAddress = GetEndpointUrl(BooruApiEndpoint.PostsApi) + tags;

            var stream = await this.HttpClient.GetStreamAsync(requestAddress);

            var result = JsonSerializer.Deserialize<ApiResponse<Post>>(stream);

            foreach (var item in result.Items)
            {
                item.PostUrl = GetEndpointUrl(BooruApiEndpoint.PostsPage) + item.Id;
            }

            return result;
        }

        private string GetEndpointUrl(BooruApiEndpoint Endpoint)
        {
            return BaseAddress + Endpoints[ServerType][Endpoint];
        }

        public async Task<ApiResponse<Post>> GetPosts(BooruPostRequestHelper requestHelper)
        {
            return await GetPosts(requestHelper.GetRequestQuery());
        }
    }

    public abstract class BooruPostRequestHelper
    {
        // Page 0 exists
        public int Page { get; set; } = 0;
        public int Limit { get; set; } = 100;
        public IEnumerable<string> Tags { get; set; }
        public bool Random { get; set; } = false;
        public BooruPostRating Rating { get; set; } = BooruPostRating.Any;

        public abstract string GetRequestQuery();
    }

    public class GelbooruPostQueryHelper : BooruPostRequestHelper
    {
        public override string GetRequestQuery()
        {
            var tags = string.Join('+', Tags);

            if (Random)
            {
                tags += "+sort:random";
            }

            switch (Rating)
            {
                case BooruPostRating.Any:
                    // No need to do anything
                    break;
                case BooruPostRating.Safe:
                    tags += "+rating:safe";
                    break;
                case BooruPostRating.SafeQuestionable:
                    tags += "-rating:explicit";
                    break;
                case BooruPostRating.Questionable:
                    tags += "+rating:questionable";
                    break;
                case BooruPostRating.QuestionableExplicit:
                    tags += "-rating:safe";
                    break;
                case BooruPostRating.Explicit:
                    tags += "+rating:explicit";
                    break;
            }

            return $"{tags}&pid={Page}&limit={Limit}";
        }
    }

    public enum BooruPostRating
    {
        Any,
        Safe,
        SafeQuestionable,
        Questionable,
        QuestionableExplicit,
        Explicit,
    }

    public enum BooruApiEndpoint
    {
        PostsApi,
        PostsPage,
        AutoComplete,
    }

    public enum BooruServer
    {
        Gelbooru,
        Safebooru,
        Rule34,
        TheBigImageboard,
        Konachan,
        Yandere,
    }
}