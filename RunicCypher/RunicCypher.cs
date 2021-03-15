using System;
using System.Text;

namespace RunicCypher
{
    public static class RunicCypher
    {
        private const string Runes = "ᚠᚡᚢᚣᚤᚥᚦᚧᚨᚩᚪᚫᚬᚭᚮᚯᚰᚱᚲᚳᚴᚵᚶᚷᚸᚹᚺᚻᚼᚽᚾᚿᛀᛁᛂᛃᛄ ,.-:";
        private const string Latin = "0123456789abcdefghijklmnñopqrstuvwxyz ,.-:";

        public static string LatinToRunic(string input)
        {
            input = input.ToLowerInvariant();
            return Transform(Latin, Runes, input);
        }

        public static string RunicToLatin(string input)
        {
            return Transform(Runes, Latin, input);
        }

        private static string Transform(string origin, string target, string input)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < input.Length; i++)
                {
                    sb.Append(target[origin.IndexOf(input[i])]);
                }
                return sb.ToString();
            }
            catch (IndexOutOfRangeException)
            {
                throw new FormatException("The input string contains invalid characters");
            }
        }
    }
}