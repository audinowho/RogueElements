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
        private readonly List<ItemRange> items;

        public RangeList()
        {
            this.items = new List<ItemRange>();
        }

        public int Count => this.items.Count;

        // TODO: Binary Search Tree

        public void Add(T spawn, Range range)
        {
            this.Erase(range);
            this.items.Add(new ItemRange(spawn, range));
        }

        public void Erase(Range range)
        {
            for (int ii = this.items.Count - 1; ii >= 0; ii--)
            {
                if (range.Min < this.items[ii].Range.Max && range.Max > this.items[ii].Range.Min)
                {
                    bool coversMin = range.Min <= this.items[ii].Range.Min;
                    bool coversMax = range.Max >= this.items[ii].Range.Max;
                    if (coversMin && coversMax)
                    {
                        this.items.RemoveAt(ii);
                    }
                    else if (coversMin)
                    {
                        this.items[ii].Range = new Range(range.Max, this.items[ii].Range.Max);
                    }
                    else if (coversMax)
                    {
                        this.items[ii].Range = new Range(this.items[ii].Range.Min, range.Min);
                    }
                    else
                    {
                        this.items.Add(new ItemRange(this.items[ii].Element, new Range(range.Max, this.items[ii].Range.Max)));
                        this.items[ii].Range = new Range(this.items[ii].Range.Min, range.Min);
                    }
                }
            }

            // TODO: exception
        }

        public void Clear()
        {
            this.items.Clear();
        }

        public bool TryGetItem(int level, out T outItem)
        {
            foreach (ItemRange item in this.items)
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
            foreach (ItemRange item in this.items)
            {
                if (item.Range.Min <= index && index < item.Range.Max)
                    return item.Element;
            }

            return default;

            // TODO: exception
        }

        [Serializable]
        private class ItemRange
        {
            public T Element;
            public Range Range;

            public ItemRange(T item, Range range)
            {
                this.Element = item;
                this.Range = range;
            }
        }
    }
}
