using System.IO;

namespace Extensions
{
    public static class ExtensionsFile
    {
        public static bool FileExistWithAnyExtension(string path, string filename)
        {
            var files = Directory.GetFiles(path);
            for (int index = 0; index < files.Length; index++)
            {
                var file = files[index];
                if (filename == Path.GetFileNameWithoutExtension(file))
                {
                    return true;
                }
            }
            return false;
        }

        public static string? FileExistWithAnyExtensionGetName(string path, string filename)
        {
            var files = Directory.GetFiles(path);
            for (int index = 0; index < files.Length; index++)
            {
                var file = files[index];
                if (filename == Path.GetFileNameWithoutExtension(file))
                {
                    return file;
                }
            }
            return null;
        }
    }
}
