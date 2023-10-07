using System;
using System.Collections.Generic;

namespace SortingAlgorithms
{
    public class InsertionSort<ItemType>
    {
        private readonly Func<ItemType, ItemType, bool> Comparison;

        private readonly LinkedList<ItemType> ResultList;

        public InsertionSort(Func<ItemType, ItemType, bool> Comparison, IEnumerable<ItemType> Origin = null)
        {
            this.ResultList = new LinkedList<ItemType>();
            this.Comparison = Comparison;

            if (Origin is not null)
            {
                foreach (var item in Origin)
                {
                    InsertNext(item);
                }
            }
        }

        public void InsertNext(ItemType item)
        {
            if (ResultList.Count == 0)
            {
                ResultList.AddLast(item);
                return;
            }

            LinkedListNode<ItemType> itemNode = ResultList.First;

            while (true)
            {
                bool current = false;

                if (itemNode.Value is ItemType currentVal)
                {
                    current = !Comparison(currentVal, item);
                }

                bool next = false;

                if (itemNode.Next is not null && itemNode.Next.Value is ItemType nextVal)
                {
                    next = !Comparison(item, nextVal);
                }

                if (current && next)
                {
                    ResultList.AddAfter(itemNode, item);
                    return;
                }
#warning probably i can optimize this.
                else if (current && !next)
                {
                    ResultList.AddBefore(itemNode, item);
                    return;
                }

                if (itemNode.Next is null)
                {
                    break;
                }
                itemNode = itemNode.Next;
            }

            ResultList.AddLast(item);
        }

        public IEnumerable<ItemType> Result => ResultList;
    }
}