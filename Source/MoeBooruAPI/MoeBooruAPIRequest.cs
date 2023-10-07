using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Web;

namespace MoeBooruApi
{
    public class MoeBooruAPIRequest
    {
        [APIBinding("tags")]
        public List<string> Tags { get; set; }
        public bool Random { get; set; }
        public MoeBooruRating Rating { get; set; }
        [APIBinding("limit")]
        public int MaxResults { get; set; }
        public MoeBooruServer TargetServer { get; set; }

        public string GetRequestUrl()
        {
            if (Tags == null)
            {
                Tags = new List<string>();
            }

            if (Random)
            {
                Tags.Add("order:random");
            }

            string rating = Rating.ToString();

            if (rating != null)
            {
                Tags.Add("rating:" + rating);
            }

            string request = Serialize();

            return ServerConverter.GetString(TargetServer) + request;
        }

        public HttpRequestMessage GetRequest()
        {
            return new HttpRequestMessage(HttpMethod.Get, GetRequestUrl());
        }

        private string Serialize()
        {
            Type thisClass = GetType();
            string request = string.Empty;

            foreach (PropertyInfo p in thisClass.GetProperties())
            {
                if(p.GetCustomAttribute<APIBinding>() is APIBinding attr)
                {
                    if (request != string.Empty)
                    {
                        request += "&";
                    }

                    object current = p.GetValue(this);

                    if (current is List<string> data)
                    {
                        request += ParseParam(attr.Name, Concat(data));
                    }
                    else
                    {
                        request += ParseParam(attr.Name, HttpUtility.UrlEncode(Convert.ToString(current)));
                    }
                }
            }
            return request;
        }

        private static string Concat(List<string> list)
        {
            string v = string.Empty;
            foreach (string o in list)
            {
                if (v != string.Empty)
                    v += "+";
                v += HttpUtility.UrlEncode(o);
            }
            return v;
        }

        private static string ParseParam(string key, string value)
        {
            return string.Format("{0}={1}", key, value);
        }

        

        private class ServerConverter
        {
            public static string GetString(MoeBooruServer server)
            {
                string value = server switch
                {
                    MoeBooruServer.Konachan => "konachan.com",
                    MoeBooruServer.Yandere => "yande.re",
                    _ => throw new Exception("Cannot Convert type Server"),
                };

                return string.Format("https://{0}/post.json?", value);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class APIBinding : Attribute
    {
        public string Name { get; }
        public APIBinding(string name)
        {
            this.Name = name;
        }
    }
}
