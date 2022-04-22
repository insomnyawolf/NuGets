using Extensions.Images;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace OsuScoreThumbnailGenerator
{
    public class ThumbnailGenerator
    {
        public static async Task<Image> CreateThumbnail(ThumbnailData ThumbnailData)
        {
            var image = await Image.LoadAsync(ThumbnailData.OriginalBackgroundPath);

            image.Mutate((context) =>
            {
                context.DrawCustomText("Patata", "Verdana", Color.DarkRed);
            });

            return image;
        }
    }

    public class ThumbnailData
    {
        public string OriginalBackgroundPath { get; set; }
    }
}