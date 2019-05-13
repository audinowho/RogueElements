// <copyright file="RangeList.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{

    [Serializable]
    public class RangeList<T>
    {
        [Serializable]
        private class ItemRange
        {
            public T Element;
            public Range Range;

            public ItemRange(T item, Range range)
            {
                Element = item;
                Range = range;
            }
        }

        private readonly List<ItemRange> items;

        public int Count { get { return items.Count; } }

        public RangeList()
        {
            items = new List<ItemRange>();
        }

        //TODO: Binary Search Tree

        public void Add(T spawn, Range range)
        {
            Erase(range);
            items.Add(new ItemRange(spawn, range));
        }

        public void Erase(Range range)
        {
            for(int ii = items.Count-1; ii >= 0; ii--)
            {
                if (range.Min < items[ii].Range.Max && range.Max > items[ii].Range.Min)
                {
                    bool coversMin = range.Min <= items[ii].Range.Min;
                    bool coversMax = range.Max >= items[ii].Range.Max;
                    if (coversMin && coversMax)
                        items.RemoveAt(ii);
                    else if (coversMin)
                        items[ii].Range = new Range(range.Max, items[ii].Range.Max);
                    else if (coversMax)
                        items[ii].Range = new Range(items[ii].Range.Min, range.Min);
                    else
                    {
                        items.Add(new ItemRange(items[ii].Element, new Range(range.Max, items[ii].Range.Max)));
                        items[ii].Range = new Range(items[ii].Range.Min, range.Min);
                    }
                }
            }
            //TODO: exception
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool TryGetItem(int level, out T outItem)
        {
            foreach (ItemRange item in items)
            {
                if (item.Range.Min <= level && level < item.Range.Max)
                {
                    outItem = item.Element;
                    return true;
                }
            }
            outItem = default;
            return false;
        }
        public T GetItem(int index)
        {
            foreach (ItemRange item in items)
            {
                if (item.Range.Min <= index && index < item.Range.Max)
                    return item.Element;
            }
            return default;
            //TODO: exception
        }


    }
}
