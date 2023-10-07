using System;

namespace Extensions
{
    public static class ExtensionsStdOutput
    {
        public static void FatalOutput(string? text)
        {
            ColoredOutput(text, "Fatal", ConsoleColor.Red);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(false);
            Environment.Exit(1);
        }

        public static void ErrorOutput(string? text)
        {
            ColoredOutput(text, "Error", ConsoleColor.Red);
        }

        public static void WarningOutput(string? text)
        {
            ColoredOutput(text, "Error", ConsoleColor.Yellow);
        }

        public static void InfoOutput(string? text)
        {
            ColoredOutput(text, "Info", ConsoleColor.White);
        }

        public static void HighLightedInfoOutput(string? text)
        {
            ColoredOutput(text, "Info", ConsoleColor.Cyan);
        }

        public static void ColoredOutput(string? text, string level, ConsoleColor Color)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = Color;
            Console.WriteLine($"{level} | Timestamp => {DateTime.Now} | Message => {text}");
            Console.ForegroundColor = oldColor;
        }
    }
}
