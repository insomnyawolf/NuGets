using System;
using System.Collections.Generic;

namespace SortingAlgorithms
{
    public static class SelectionSort
    {
        public static void Sort<ItemType>(IList<ItemType> list, Func<ItemType, ItemType, bool> condition)
        {
            var currentOrderIndex = list.Count;

            // do till everything is in order
            while (currentOrderIndex > 0)
            {
                int tempIndex = 0;

                // Don't check already ordered patterns
                for (int listIndex = 0; listIndex < currentOrderIndex; listIndex++)
                {
                    if (!condition(list[listIndex], list[tempIndex]))
                    {
                        tempIndex = listIndex;
                    }
                }

                // At least the last number should be ordered so you don't need to check it again
                currentOrderIndex--;

                // Prevents extra work when the selected position and the position being ordered are already in order
                if (tempIndex < currentOrderIndex)
                {
                    var tempValue = list[tempIndex];
                    list[tempIndex] = list[currentOrderIndex];
                    list[currentOrderIndex] = tempValue;
                }
            }
        }
    }
}