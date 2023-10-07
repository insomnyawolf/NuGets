using System;
using System.Collections.Generic;

namespace SortingAlgorithms
{
    public static partial class Sort
    {
        private static readonly Random rng = new();

        public static void RandomSort<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
#pragma warning disable IDE0180 // Use tuple to swap values
                // Doing it that way allocates less memory, see for info https://github.com/dotnet/roslyn/issues/50784
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
#pragma warning restore IDE0180 // Use tuple to swap values+
            }
        }
    }
}
