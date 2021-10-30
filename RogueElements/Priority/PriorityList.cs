// <copyright file="PriorityList.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Stores and retrieves values with an associated priority, abstracting out the list-of-lists logic behind them.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PriorityList<T> : IPriorityList<T>, ICollection<KeyValuePair<Priority, T>>
    {
        private readonly Dictionary<Priority, List<T>> dict;

        public PriorityList()
        {
            this.dict = new Dictionary<Priority, List<T>>();
        }

        /// <summary>
        /// Retrieves the total amount of priorities being occupied with items.
        /// </summary>
        public int PriorityCount => this.dict.Count;

        /// <summary>
        /// Retrieves the total number of items in the PriorityList
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                foreach (Priority priority in this.dict.Keys)
                    count += this.dict[priority].Count;
                return count;
            }
        }

        bool ICollection<KeyValuePair<Priority, T>>.IsReadOnly => false;

        public void Add(int priority, T item)
        {
            this.Add(new Priority(priority), item);
        }

        public void Add(Priority priority, T item)
        {
            if (!this.dict.ContainsKey(priority))
                this.dict[priority] = new List<T>();
            this.dict[priority].Add(item);
        }

        void ICollection<KeyValuePair<Priority, T>>.Add(KeyValuePair<Priority, T> item) => this.Add(item.Key, item.Value);

        void IPriorityList.Add(Priority priority, object item) => this.Add(priority, (T)item);

        public void Insert(int priority, int index, T item)
        {
            this.Insert(new Priority(priority), index, item);
        }

        public void Insert(Priority priority, int index, T item)
        {
            if (!this.dict.ContainsKey(priority))
            {
                if (index != 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of bounds of the list.");
                this.dict[priority] = new List<T>();
            }

            this.dict[priority].Insert(index, item);
        }

        void IPriorityList.Insert(Priority priority, int index, object item) => this.Insert(priority, index, (T)item);

        public void RemoveAt(Priority priority, int index)
        {
            this.dict[priority].RemoveAt(index);
            if (this.dict[priority].Count == 0)
                this.dict.Remove(priority);
        }

        bool ICollection<KeyValuePair<Priority, T>>.Remove(KeyValuePair<Priority, T> item)
        {
            List<T> val;
            if (this.dict.TryGetValue(item.Key, out val))
                return val.Remove(item.Value);
            return false;
        }

        public T Get(Priority priority, int index)
        {
            return this.dict[priority][index];
        }

        bool ICollection<KeyValuePair<Priority, T>>.Contains(KeyValuePair<Priority, T> item)
        {
            List<T> val;
            if (this.dict.TryGetValue(item.Key, out val))
                return val.Contains(item.Value);
            return false;
        }

        object IPriorityList.Get(Priority priority, int index) => this.Get(priority, index);

        public void Set(Priority priority, int index, T item)
        {
            this.dict[priority][index] = item;
        }

        void IPriorityList.Set(Priority priority, int index, object item) => this.Set(priority, index, (T)item);

        public void Clear()
        {
            this.dict.Clear();
        }

        /// <summary>
        /// Enumerates all priorities. Returns in order.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Priority> GetPriorities()
        {
            List<Priority> priorities = new List<Priority>();
            foreach (Priority key in this.dict.Keys)
                priorities.Add(key);

            priorities.Sort();

            foreach (Priority key in priorities)
                yield return key;
        }

        public IEnumerable<T> GetItems(Priority priority)
        {
            foreach (T item in this.dict[priority])
                yield return item;
        }

        IEnumerable IPriorityList.GetItems(Priority priority) => this.GetItems(priority);

        /// <summary>
        /// Enumerates all items. Does so in priority order.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> EnumerateInOrder()
        {
            foreach (Priority key in this.GetPriorities())
            {
                foreach (T item in this.dict[key])
                    yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.EnumerateKeyValuePairs();

        public int GetCountAtPriority(Priority priority)
        {
            if (this.dict.TryGetValue(priority, out List<T> items))
                return items.Count;
            return 0;
        }

        void ICollection<KeyValuePair<Priority, T>>.CopyTo(KeyValuePair<Priority, T>[] array, int arrayIndex)
        {
            foreach (Priority key in this.GetPriorities())
            {
                foreach (T item in this.dict[key])
                {
                    array[arrayIndex] = new KeyValuePair<Priority, T>(key, item);
                    arrayIndex++;
                }
            }
        }

        IEnumerator<KeyValuePair<Priority, T>> IEnumerable<KeyValuePair<Priority, T>>.GetEnumerator()
        {
            return this.EnumerateKeyValuePairs();
        }

        int IPriorityList.GetCountAtPriority(Priority priority) => this.GetCountAtPriority(priority);

        private IEnumerator<KeyValuePair<Priority, T>> EnumerateKeyValuePairs()
        {
            foreach (Priority key in this.GetPriorities())
            {
                foreach (T item in this.dict[key])
                    yield return new KeyValuePair<Priority, T>(key, item);
            }
        }
    }
}
