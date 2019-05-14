// <copyright file="PriorityList.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class PriorityList<T> : IPriorityList<T>
    {
        private readonly Dictionary<int, List<T>> data;

        public PriorityList()
        {
            this.data = new Dictionary<int, List<T>>();
        }

        public int PriorityCount => this.data.Count;

        public int Count
        {
            get
            {
                int count = 0;
                foreach (int priority in this.data.Keys)
                    count += this.data[priority].Count;
                return count;
            }
        }

        public void Add(int priority, T item)
        {
            if (!this.data.ContainsKey(priority))
                this.data[priority] = new List<T>();
            this.data[priority].Add(item);
        }

        void IPriorityList.Add(int priority, object item) => this.Add(priority, (T)item);

        public void Insert(int priority, int index, T item)
        {
            if (!this.data.ContainsKey(priority))
            {
                if (index != 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of bounds of the list.");
                this.data[priority] = new List<T>();
            }

            this.data[priority].Insert(index, item);
        }

        void IPriorityList.Insert(int priority, int index, object item) => this.Insert(priority, index, (T)item);

        public void RemoveAt(int priority, int index)
        {
            this.data[priority].RemoveAt(index);
            if (this.data[priority].Count == 0)
                this.data.Remove(priority);
        }

        public T Get(int priority, int index)
        {
            return this.data[priority][index];
        }

        object IPriorityList.Get(int priority, int index) => this.Get(priority, index);

        public void Set(int priority, int index, T item)
        {
            this.data[priority][index] = item;
        }

        void IPriorityList.Set(int priority, int index, object item) => this.Set(priority, index, (T)item);

        public void Clear()
        {
            this.data.Clear();
        }

        public IEnumerable<int> GetPriorities()
        {
            foreach (int key in this.data.Keys)
                yield return key;
        }

        public IEnumerable<T> GetItems(int priority)
        {
            foreach (T item in this.data[priority])
                yield return item;
        }

        IEnumerable IPriorityList.GetItems(int priority) => this.GetItems(priority);

        public IEnumerator<T> GetEnumerator()
        {
            foreach (int key in this.data.Keys)
            {
                foreach (T item in this.data[key])
                    yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public int GetCountAtPriority(int priority)
        {
            if (this.data.TryGetValue(priority, out List<T> items))
                return items.Count;
            return 0;
        }

        int IPriorityList.GetCountAtPriority(int priority) => this.GetCountAtPriority(priority);
    }
}
