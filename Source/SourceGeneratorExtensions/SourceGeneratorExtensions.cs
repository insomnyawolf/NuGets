using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SourceGeneratorExtensions
{
    public static class SourceGeneratorExtensions
    {
        public static bool HasToken(this SyntaxTokenList tokenList, string tokenValue)
        {
            for (int i = 0; i < tokenList.Count; i++)
            {
                var item = tokenList[i];

                if (item.Text == tokenValue)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool InheritFrom(this INamedTypeSymbol symbol, string fullyQualifiedName)
        {
            while (symbol.BaseType is null == false)
            {
                symbol = symbol.BaseType;

                var name = symbol.ToString();

                if (name == fullyQualifiedName)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasInterface(this ITypeSymbol symbol, string name)
        {
            return symbol.AllInterfaces.Any(i => i.MetadataName == name);
        }

        public static bool IsEnumerable(this ITypeSymbol symbol)
        {
            return symbol.HasInterface(typeof(IEnumerable<>).Name);
        }

        public static bool IsDictionary(this ITypeSymbol symbol)
        {
            return symbol.HasInterface(typeof(IDictionary<,>).Name);
        }

        public static bool IsPartiallyUdaptableClass(this ITypeSymbol symbol)
        {
            return symbol.TypeKind == TypeKind.Class
                && symbol.GetFullyQualifiedName() != "System.String"
                && !symbol.IsEnumerable();
        }

        public static string GetFullyQualifiedName(this ISymbol symbol)
        {
            return $"{symbol.ContainingNamespace}.{symbol.Name}";
        }

        public static bool HasAttribute(this ITypeSymbol typeSymbol, string name)
        {
            var attrs = typeSymbol.GetAttributes();
            foreach (var attr in attrs)
            {
                if (attr.AttributeClass?.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        public static string GetFullyQualifiedGenericsString(this INamedTypeSymbol symbol)
        {
            var @base = symbol.GetFullyQualifiedName();

            var typeArguments = symbol.TypeArguments;

            var genericStringComponents = new List<string>();

            foreach (var type in typeArguments)
            {
                if (type is INamedTypeSymbol subSymbol)
                {
                    var subgenerics = subSymbol.GetFullyQualifiedGenericsString();

                    if (subgenerics.Length > 0)
                    {
                        genericStringComponents.Add(subgenerics);
                    }
                }
            }

            if (genericStringComponents.Count < 1)
            {
                return @base;
            }

            var joined = string.Join(", ", genericStringComponents);

            return $"{@base}<{joined}>";
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
