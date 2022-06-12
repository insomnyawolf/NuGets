using System.IO;
using System.IO.Compression;

namespace InMemoryDatabase
{
    public static class StoredDbCompression
    {
        public static void CompressTo(this Stream Origin, Stream Target)
        {
            using (GZipStream compressionStream = new GZipStream(Target, CompressionMode.Compress))
            {
                Origin.CopyTo(compressionStream);
                compressionStream.Flush();
            }
        }

        public static void DecompressTo(this Stream Origin, Stream Target)
        {
            using (GZipStream compressionStream = new GZipStream(Target, CompressionMode.Decompress))
            {
                Origin.CopyTo(compressionStream);
                compressionStream.Flush();
            }
        }
    }
}
