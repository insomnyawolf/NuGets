namespace Extensions
{
    public static class Format
    {
        private static readonly string[] ExtensionesImagen = new string[]
        {
            ".png",
            ".jpg",
            ".gif",
            ".webp",
            ".bmp",
        };

        public static bool HasImageExtension(this string uri)
        {
            uri = uri.ToLower();

            for (int i = 0; i < ExtensionesImagen.Length; i++)
            {
                if (uri.EndsWith(ExtensionesImagen[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
