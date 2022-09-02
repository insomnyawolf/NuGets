using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace SaucenaoSearch
{
    // Replacing api with web scrapping

    // https://saucenao.com/user.php?page=search-api

    public class SaucenaoWebInterface
    {
        private const string SaucenaoHost = "http://saucenao.com/search.php";
        private readonly HttpClient HttpClient;

        public SaucenaoWebInterface(HttpClient? HttpClient = null)
        {
            this.HttpClient = HttpClient ?? new HttpClient();
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

                    var result = ParseResult(tableNode);

                    list.Add(result);
                }
            }

            return list;
        }



        private static SaucenaoResponse ParseResult(HtmlNode currentResult)
        {
            var parsedResponse = new SaucenaoResponse();

            foreach (var groupInfoNode in currentResult.FirstChild.LastChild.ChildNodes)
            {
                if (groupInfoNode.HasClass("resultmatchinfo"))
                {
                    ParseInfo(parsedResponse, groupInfoNode);
                    continue;
                }

                if (groupInfoNode.HasClass("resultcontent"))
                {
                    foreach (var infoNode in groupInfoNode.ChildNodes)
                    {
                        ParseSource(parsedResponse, infoNode);
                    }
                }
            }

            return parsedResponse;
        }

        private static void ParseInfo(SaucenaoResponse result, HtmlNode currentNode)
        {
            foreach (var infoNode in currentNode.ChildNodes)
            {
                if (infoNode.HasClass("resultsimilarityinfo"))
                {
                    var text = infoNode.InnerText.Replace("%", "");
                    result.Similarity = float.Parse(text);
                    continue;
                }

                if (infoNode.HasClass("resultmiscinfo"))
                {
                    foreach (var misc in infoNode.ChildNodes)
                    {
                        if (misc.Name == "a")
                        {
                            var href = misc.GetAttributeValue<string>("href", null);

                            if (!string.IsNullOrEmpty(href))
                            {
                                result.OtherUrls.Add(href.Trim());
                            }
                        }
                    }
                }
            }
        }

        private static void ParseSource(SaucenaoResponse result, HtmlNode currentNode)
        {
            if (currentNode.HasClass("resulttitle"))
            {
                const string patternCreator = "Creator:";
                if (currentNode.InnerText.StartsWith(patternCreator))
                {
                    result.Creator = currentNode.InnerText[patternCreator.Length..];
                    return;
                }

                // booru like sites seems to use that field to put the creator's name
                // it also doesn't seem to have the title anywhere

                result.Title = currentNode.InnerText;
                return;
            }

            if (currentNode.HasClass("resultcontentcolumn"))
            {
                foreach (var contentColumn in currentNode.ChildNodes)
                {
#warning improve this

                    // ArtStation
                    if (contentColumn.InnerText.StartsWith("ArtStation Project:"))
                    {
                        result.SourceUrl = contentColumn.NextSibling.GetAttributeValue<string>("href", null)?.Trim();
                        continue;
                    }

                    // ArtStation
                    if (contentColumn.InnerText.StartsWith("Author:"))
                    {
                        result.Creator = contentColumn.NextSibling.InnerText;
                        continue;
                    }

                    // Pixiv
                    if (contentColumn.InnerText.StartsWith("Pixiv ID:"))
                    {
                        result.SourceUrl = contentColumn.NextSibling.GetAttributeValue<string>("href", null)?.Trim();
                        continue;
                    }

                    // Pixiv
                    if (contentColumn.InnerText.StartsWith("Member:"))
                    {
                        result.Creator = contentColumn.NextSibling.InnerText;
                        continue;
                    }

                    // Booru
                    if (contentColumn.InnerText.StartsWith("Source:"))
                    {
                        result.SourceUrl = contentColumn.NextSibling.GetAttributeValue<string>("href", null)?.Trim();
                        continue;
                    }

                    // Booru
                    if (contentColumn.InnerText.StartsWith("Creator:"))
                    {
                        result.Creator = contentColumn.NextSibling.InnerText;
                        continue;
                    }

                    // Twitter
                    if (contentColumn.InnerText.StartsWith("Tweet ID:"))
                    {
                        result.SourceUrl = contentColumn.NextSibling.GetAttributeValue<string>("href", null)?.Trim();
                        continue;
                    }

                    // Twitter
                    if (contentColumn.InnerText.StartsWith("Twitter:"))
                    {
                        result.Creator = contentColumn.NextSibling.InnerText;
                        continue;
                    }
                }
            }
        }
    }
}
