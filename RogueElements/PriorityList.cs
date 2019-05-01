using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class PriorityList<T> : IEnumerable<T>, IEnumerable
    {
        private Dictionary<int, List<T>> data;

        public PriorityList()
        {
            data = new Dictionary<int, List<T>>();
        }

        public void Add(int priority, T item)
        {
            if (!data.ContainsKey(priority))
                data[priority] = new List<T>();
            data[priority].Add(item);
        }

        public void Insert(int priority, int index, T item)
        {
            data[priority].Insert(index, item);
        }

        public void RemoveAt(int priority, int index)
        {
            data[priority].RemoveAt(index);
        }

        public T Get(int priority, int index)
        {
            return data[priority][index];
        }

        public void Clear()
        {
            data.Clear();
        }

        public int GetCountAtPriority(int priority)
        {
            return data[priority].Count;
        }


        public IEnumerable<int> GetPriorities()
        {
            foreach(int key in data.Keys)
                yield return key;
        }

        public IEnumerable<T> GetItems(int priority)
        {
            foreach (T item in data[priority])
                yield return item;
        }


        public IEnumerator<T> GetEnumerator()
        {
            foreach (int key in data.Keys)
            {
                foreach (T item in data[key])
                    yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public int PriorityCount { get { return data.Count; } }

        public int Count
        {
            get
            {
                int count = 0;
                foreach (int priority in data.Keys)
                    count += data[priority].Count;
                return count;
            }
        }
    }

}

