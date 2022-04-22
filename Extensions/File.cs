using System.IO;

namespace Extensions.File
{
    public static class ExtensionsFile
    {
        public static bool FileExistWithAnyExtension(string path, string filename)
        {
            var exists = false;
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (filename == Path.GetFileNameWithoutExtension(file))
                {
                    exists = true;
                    break;
                }
            }
            return exists;
        }

        public static string? FileExistWithAnyExtensionGetName(string path, string filename)
        {
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                if (filename == Path.GetFileNameWithoutExtension(file))
                {
                    return file;
                }
            }
            return null;
        }
    }
}
