using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SourceGeneratorExtensions
{
    public static class FileTemplateExtensions
    {
        private static readonly Type AssemblyType = typeof(SourceGeneratorExtensions);

        public static Stream GetEmbedFile(string filename)
        {
            const string folderName = "EmbedResources";
            var path = $"{AssemblyType.Namespace}.{folderName}.{filename}";
            var resource = AssemblyType.Assembly.GetManifestResourceStream(path);
            return resource;
        }

        private static readonly Dictionary<string, string> FileCache = new Dictionary<string, string>();

        public static string GetEmbedFileAsString(string filename)
        {
            if (FileCache.TryGetValue(filename, out var value))
            {
                return value;
            }

            string text;
            using (var embedFile = GetEmbedFile(filename))
            {
                using (var reader = new StreamReader(embedFile))
                {
                    text = reader.ReadToEnd();

                    FileCache.Add(filename, text);
                }

            }

            return text;
        }

        public static string FillTemplateFromFile(string filename, Dictionary<string, string> replacements)
        {
            var template = GetEmbedFileAsString(filename);

            return FillTemplate(template, replacements);
        }

        public static string FillTemplate(string template, Dictionary<string, string> replacements)
        {
            return Regex.Replace(template, @"Template(.+?)Template", m =>
            {
                if (!replacements.TryGetValue(m.Groups[1].Value, out var value))
                {
                    return "ERROR PATTERN NOT FOUND";
                }

                return value;
            });
        }

        public static void AddStaticFile(this IncrementalGeneratorPostInitializationContext context, string filename)
        {
            var templateString = GetEmbedFileAsString(filename);

            var outputName = filename.Replace(".cs", ".generated.cs");

            context.AddSource(outputName, templateString);
        }

        public static void AddTemplate(this SourceProductionContext context, string filename, string discriminator, Dictionary<string, string> replacements)
        {
            var source = FillTemplateFromFile(filename, replacements);

            if (!(discriminator is null))
            {
                filename = GetDiscriminatedName(filename, discriminator);
            }

            context.AddSource(filename, source);
        }

        public static string GetDiscriminatedName(string filename, string discriminator)
        {
            discriminator = string.Concat(discriminator.Split(Path.GetInvalidFileNameChars()));

            filename = filename.Replace(".cs", $".{discriminator}.cs");
            filename = filename.Replace(".cs", ".generated.cs");

            return filename;
        }
    }
}
