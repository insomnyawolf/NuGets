﻿using BooruApi.Models;
using BooruApi.Models.Gelbooru;
using BooruApi.Models.Gelbooru.PostEndpoint;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BooruApi
{
    // https://github.com/insomnyawolf/Gonnachan/blob/master/const.go
    public abstract class ApiBase<T>
    {
        public abstract string BaseUrl { get; }
        public abstract string PostEndpoint { get; }
        public abstract string TagEndpoint { get; }
        public abstract string PostPage { get; }
        public abstract string AutoComplete { get; }
        private HttpClient HttpClient { get; set; }

        //private static readonly Dictionary<BooruServer, string> ServerAdresses = new()
        //{
        //    { BooruServer.Safebooru, "https://safebooru.org/index.php?page=dapi&q=index&json=1&s=post&" },
        //    { BooruServer.Rule34, "https://rule34.xxx/index.php?page=dapi&q=index&json=1&s=post&" },
        //    { BooruServer.TheBigImageboard, "https://tbib.org/index.php?page=dapi&q=index&json=1&s=post&" },
        //    { BooruServer.Konachan, "https://konachan.com/post.json?" },
        //    { BooruServer.Yandere, "https://yande.re/post.json?" },
        //};

        public ApiBase(HttpClient? HttpClient = null)
        {
            this.HttpClient = HttpClient ?? new HttpClient();
        }

        public string GetAutoCompleteUrl(string input)
        {
            return AutoComplete + input;
        }

        public async Task<List<AutoCompleteResponse>> GetAutoCompleteAsync(string input)
        {
            var requestAddress = GetAutoCompleteUrl(input);

            var stream = await this.HttpClient.GetStreamAsync(requestAddress);

            return JsonSerializer.Deserialize<List<AutoCompleteResponse>>(stream);
        }

        public string GetPostsUrl(string tags)
        {
            return PostEndpoint + tags;
        }

        public string GetPostsUrl(T queryHelper)
        {
            return GetPostsUrl(queryHelper.ToString());
        }

        public async Task<ApiResponsePost> GetPostsAsync(string tags)
        {
            var requestAddress = GetPostsUrl(tags);

            var stream = await this.HttpClient.GetStreamAsync(requestAddress);

            var result = JsonSerializer.Deserialize<ApiResponsePost>(stream);

            for (int i = 0; i < result.Posts.Count; i++)
            {
                var post = result.Posts[i];
                post.PostUrl = PostPage + post.Id;
            }

            return result;
        }

        public async Task<ApiResponsePost> GetPostsAsync(T requestHelper)
        {
            return await GetPostsAsync(requestHelper.ToString());
        }
    }

    public abstract class PostRequestHelper
    {
        // Page 0 exists
        public virtual int Page { get; set; } = 0;
        public virtual int Limit { get; set; } = 100;
        public virtual IEnumerable<Tag>? Tags { get; set; }
        public virtual Sort? Sort { get; set; }
        public virtual RangeValue? Score { get; set; }
        public virtual Size? Size { get; set; }
        public virtual Rating? Rating { get; set; }
        public override string ToString()
        {
            var tags = $"&pid={Page}&limit={Limit}&tags=";

            if (Tags != null)
            {
                tags += string.Join(string.Empty, Tags);
            }

            if (Sort != null)
            {
                tags += Sort;
            }

            if (Score != null)
            {
                tags += Score;
            }

            if (Size != null)
            {
                tags += Size;
            }

            if (Rating != null)
            {
                tags += Rating;
            }

            return tags;
        }
    }

    public class Tag
    {
        public virtual string Value { get; set; }
        public SearchType SearchType { get; set; } = SearchType.Include;

        public override string ToString()
        {
            switch (SearchType)
            {
                case SearchType.Include:
                    return "+" + Value;
                case SearchType.Exclude:
                default:
                    return "-" + Value;
            }
        }
    }

    public class Score : RangeValue
    {
        public override string ToString()
        {
            return $"+score:{base.ToString()}";
        }
    }

    public class Size
    {
        public virtual RangeValue Width { get; set; }
        public virtual RangeValue Height { get; set; }

        public override string ToString()
        {
            var value = "";

            if (Width != null)
            {
                value += $"+width:{Width}";
            }

            if (Height != null)
            {
                value += $"+height:{Height}";
            }

            return value;
        }
    }

    public class Sort
    {
        public virtual SortType Type { get; set; }
        public virtual SortDirection Direction { get; set; }
        public virtual int? RandomSeed { get; set; }

        public override string ToString()
        {
            var result = "+sort:";

            result += Type switch
            {
                SortType.Id => "id",
                SortType.Score => "score",
                SortType.Rating => "rating",
                SortType.User => "user",
                SortType.Height => "height",
                SortType.Width => "width",
                SortType.Source => "source",
                SortType.Updated => "updated",
                SortType.Random => "random",
            };

            if (Type == SortType.Random && RandomSeed.HasValue)
            {
                result += $":{RandomSeed}";
            }
            else
            {
                result += Direction switch
                {
                    SortDirection.Asc => ":asc",
                    SortDirection.Desc => ":desc",
                };
            }

            return result;
        }
    }

    public class RangeValue
    {
        public virtual CompareType CompareType { get; set; }
        public virtual int Value { get; set; }

        public override string ToString()
        {
            return CompareType switch
            {
                CompareType.Smaller => $"<{Value}",
                CompareType.SmallerEqual => $"<={Value}",
                CompareType.Equal => $"={Value}",
                CompareType.EqualGreater => $">={Value}",
                CompareType.Greater => $">{Value}",
            };
        }
    }

    public class Rating
    {
        public virtual PostRating? PostRating { get; set; }

        public override string ToString()
        {
            switch (PostRating)
            {
                case BooruApi.PostRating.Explicit:
                    return "+rating:explicit";
                case BooruApi.PostRating.QuestionableExplicit:
                    return "-rating:safe";
                case BooruApi.PostRating.Questionable:
                    return "+rating:questionable";
                case BooruApi.PostRating.SafeQuestionable:
                    return "-rating:explicit";
                case BooruApi.PostRating.Safe:
                    return "+rating:safe";
                default:
                    return "";
            }
        }
    }

    public enum CompareType
    {
        Smaller,
        SmallerEqual,
        Equal,
        EqualGreater,
        Greater,
    }

    public enum PostRating
    {
        Safe,
        SafeQuestionable,
        Questionable,
        QuestionableExplicit,
        Explicit,
    }

    public enum SortDirection
    {
        Asc,
        Desc,
    }

    public enum SortType
    {
        Id,
        Score,
        Rating,
        User,
        Height,
        Width,
        Source,
        Updated,
        Random,
    }

    public enum SearchType
    {
        Include,
        Exclude,
    }
}