using SaucenaoApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SaucenaoApi
{
    // https://saucenao.com/user.php?page=search-api
    public class SaucenaoConfig
    {
        public string ApiKey { get; set; }

        // 0 normal html
        // 1 xml api(not implemented)
        // 2 json api
        public int OutputType { get; set; } = 2;

        // 0 Does nothing
        // 1 Causes each index which has a match to output at most 1 for testing. Works best with a numres greater than the number of indexes searched.
        public string TestMode { get; set; }
        public string Dbmask { get; set; }
        public string Dbmaski { get; set; }
        public string Db { get; set; }
        public string Numres { get; set; }

        //0 no result deduping
        //1 consolidate booru results and dedupe by item identifier
        //2 all implemented dedupe methods such as by series name.
        // Default is 2, more levels may be added in future.
        public string Dedupe { get; set; }
        public string Minsim { get; set; }
    }

    public class Saucenao
    {
        private const string SaucenaoHost = "http://saucenao.com/search.php?";
        private readonly HttpClient HttpClient;
        private readonly SaucenaoConfig SaucenaoRequesterConfig;

        public Saucenao(HttpClient HttpClient, SaucenaoConfig SaucenaoRequesterConfig)
        {
            this.HttpClient = HttpClient;
            this.SaucenaoRequesterConfig = SaucenaoRequesterConfig;
        }

        public Saucenao(SaucenaoConfig SaucenaoRequesterConfig) : this(new HttpClient(), SaucenaoRequesterConfig)
        {

        }

        private Dictionary<string, object> GetRequestBase()
        {
            return new Dictionary<string, object>()
            {
                { "api_key", SaucenaoRequesterConfig.ApiKey },
                { "output_type", SaucenaoRequesterConfig.OutputType },
                { "testmode", SaucenaoRequesterConfig.TestMode },
                { "dbmask", SaucenaoRequesterConfig.Dbmask },
                { "dbmaski", SaucenaoRequesterConfig.Dbmaski },
                { "db", SaucenaoRequesterConfig.Db },
                { "numres", SaucenaoRequesterConfig.Numres },
                { "dedupe", SaucenaoRequesterConfig.Dedupe },
                { "minsim", SaucenaoRequesterConfig.Minsim },
            };
        }

        //Example output
        // https://saucenao.com/search.php?db=999&output_type=2&testmode=1&numres=16&url=http%3A%2F%2Fsaucenao.com%2Fimages%2Fstatic%2Fbanner.gif
        private static Uri SerializeUrl(Dictionary<string, object> values)
        {
            var query = string.Empty;

            foreach (var item in values)
            {
                if (item.Value is null)
                {
                    continue;
                }

                query += $"{item.Key}={item.Value}&";
            }

            return new Uri(SaucenaoHost + query);
        }

        public async Task<SaucenaoResponse> FindSource(Stream image)
        {
            var data = GetRequestBase();

            var url = SerializeUrl(data);

            var req = new HttpRequestMessage(HttpMethod.Post, url);

            // Add Image Stream Here
            using var content = new MultipartFormDataContent("Upload----" + DateTime.Now.Ticks.ToString("x"))
            {
                { new StreamContent(image), "file", "upload.jpg" }
            };

            req.Content = content;

            return await FindSource(req);
        }

        public async Task<SaucenaoResponse> FindSource(string imageUrl)
        {
            var data = GetRequestBase();

            data.Add("url", imageUrl);

            var url = SerializeUrl(data);

            var req = new HttpRequestMessage(HttpMethod.Post, url);

            return await FindSource(req);
        }

        private async Task<SaucenaoResponse> FindSource(HttpRequestMessage httpRequestMessage)
        {
            var response = await HttpClient.SendAsync(httpRequestMessage);

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(content);
            }

            content = Regex.Unescape(content).Replace("\\/", "/");

            return JsonSerializer.Deserialize<SaucenaoResponse>(content);
        }
    }
}
