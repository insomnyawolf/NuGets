namespace BooruApi
{
    // https://github.com/insomnyawolf/Gonnachan/blob/master/const.go
    public class BooruApiRequest
    {
        public static readonly Dictionary<BooruServer, string> ServerAdresses = new()
        {
            { BooruServer.Gelbooru, "https://gelbooru.com/index.php?page=dapi&q=index&json=1&s=post&" },
            { BooruServer.Safebooru, "https://safebooru.org/index.php?page=dapi&q=index&json=1&s=post&" },
            { BooruServer.Rule34, "https://rule34.xxx/index.php?page=dapi&q=index&json=1&s=post&" },
            { BooruServer.TheBigImageboard, "https://tbib.org/index.php?page=dapi&q=index&json=1&s=post&" },
            { BooruServer.Konachan, "https://konachan.com/post.json?" },
            { BooruServer.Yandere, "https://yande.re/post.json?" },
            //{ BooruServer.Sankaku, "https://capi-beta.sankakucomplex.com/post/index.json?page=1&" },
        };
    }

    public enum BooruApiEndpoint
    {
        Posts,
    }

    public enum BooruServer
    {
        Konachan,
        Yandere,
        Gelbooru,
        Rule34,
        Safebooru,
        TheBigImageboard,
        //Sankaku,
    }
}