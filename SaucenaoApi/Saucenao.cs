using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SaucenaoApi
{
    // Replacing api with web scrapping

    // https://saucenao.com/user.php?page=search-api

    public class Saucenao
    {
        private const string SaucenaoHost = "http://saucenao.com/search.php";
        private readonly HttpClient HttpClient;

        public Saucenao(HttpClient HttpClient)
        {
            this.HttpClient = HttpClient;
        }

        public Saucenao() : this(new HttpClient())
        {

        }

        public async Task<List<SaucenaoResponse>> GetSauceAsync(Stream file, string filename)
        {
            var data = GetBaseHttpRequestMessage();

            data.Add(new StreamContent(file), "file", filename);

            return await GetSauceAsync(data);
        }

        public async Task<List<SaucenaoResponse>> GetSauceAsync(string url)
        {
            var data = GetBaseHttpRequestMessage();

            data.Add(new StringContent(url), "url");

            return await GetSauceAsync(data);
        }

        private MultipartFormDataContent GetBaseHttpRequestMessage()
        {
            var req = new MultipartFormDataContent
            {
                { new StringContent("1"), "frame" },
                { new StringContent("0"), "hide" },
                { new StringContent("999"), "database" }
            };

            return req;
        }

        private async Task<List<SaucenaoResponse>> GetSauceAsync(HttpContent httpRequestMessage)
        {
            var response = await HttpClient.PostAsync(SaucenaoHost, httpRequestMessage);

            var content = await response.Content.ReadAsStreamAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(await new StreamReader(content).ReadToEndAsync());
            }

            var doc = new HtmlDocument();

            doc.Load(content);

            var nodes = doc.DocumentNode.SelectSingleNode("/html/body/div[2]/div[3]");

            var list = new List<SaucenaoResponse>();

            foreach (var resultNodes in nodes.ChildNodes)
            {
                // Pilla solo los resultados
                if (!resultNodes.HasClass("result"))
                {
                    continue;
                }

                // ignora los de baja similitud
                if (resultNodes.HasClass("hidden"))
                {
                    continue;
                }

                foreach (var tableNode in resultNodes.ChildNodes)
                {
                    // ignora el botón de mostrar resultados de baja similitud
                    if (tableNode.NodeType != HtmlNodeType.Element)
                    {
                        continue;
                    }

                    var parsedResponse = new SaucenaoResponse();

                    foreach (var groupInfoNode in tableNode.FirstChild.LastChild.ChildNodes)
                    {
                        if (groupInfoNode.HasClass("resultmatchinfo"))
                        {
                            foreach (var infoNode in groupInfoNode.ChildNodes)
                            {
                                if (infoNode.HasClass("resultsimilarityinfo"))
                                {
                                    parsedResponse.Similarity = infoNode.InnerText;
                                    continue;
                                }

                                if (infoNode.HasClass("resultmiscinfo"))
                                {
                                    foreach (var misc in infoNode.ChildNodes)
                                    {
                                        if(misc.Name == "a")
                                        {
                                            var href = misc.GetAttributeValue<string>("href", null);

                                            if (!string.IsNullOrEmpty(href))
                                            {
                                                parsedResponse.Url.Add(href.Trim());
                                            }
                                            continue;
                                        }
                                    }
                                }
                            }

                        }

                        if (groupInfoNode.HasClass("resultcontent"))
                        {
                            foreach (var infoNode in groupInfoNode.ChildNodes)
                            {
                                if (infoNode.HasClass("resulttitle"))
                                {
                                    const string patternCreator = "Creator:";
                                    if (infoNode.InnerText.StartsWith(patternCreator))
                                    {
                                        parsedResponse.Creator = infoNode.InnerText[patternCreator.Length..];
                                        continue;
                                    }

                                    parsedResponse.Title = infoNode.InnerText;
                                    continue;
                                }

                                if (infoNode.HasClass("resultcontentcolumn"))
                                {
                                    foreach (var contentColumn in infoNode.ChildNodes)
                                    {
                                        const string patternSource = "Source:";
                                        if (contentColumn.InnerText.StartsWith(patternSource))
                                        {
                                            parsedResponse.SourceUrl = contentColumn.NextSibling.GetAttributeValue<string>("href", null)?.Trim();
                                            continue;
                                        }

                                        const string patternPixivId = "Pixiv ID:";
                                        if (contentColumn.InnerText.StartsWith(patternPixivId))
                                        {
                                            parsedResponse.SourceUrl = contentColumn.NextSibling.GetAttributeValue<string>("href", null)?.Trim();
                                            continue;
                                        }

                                        const string patterCreator2 = "Member:";
                                        if (contentColumn.InnerText.StartsWith(patterCreator2))
                                        {
                                            parsedResponse.Creator = contentColumn.NextSibling.InnerText;
                                            continue;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    list.Add(parsedResponse);
                }

            }

            return list;
        }
    }

    public class SaucenaoResponse
    {
        public string Similarity { get; set; }
        public List<string> Url { get; set; } = new List<string>();
        public string SourceUrl { get; set; }
        public string Title { get; set; }
        public string Creator { get; set; }
    }
}
