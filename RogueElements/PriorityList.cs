using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class PriorityList<T> : IEnumerable<T>, IPriorityList<T>
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
        void IPriorityList.Add(int priority, object item) { Add(priority, (T)item); }

        public void Insert(int priority, int index, T item)
        {
            if (!data.ContainsKey(priority))
            {
                if (index != 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index was out of bounds of the list.");
                data[priority] = new List<T>();
            }
            data[priority].Insert(index, item);
        }
        void IPriorityList.Insert(int priority, int index, object item) { Insert(priority, index, (T)item); }

        public void RemoveAt(int priority, int index)
        {
            data[priority].RemoveAt(index);
            if (data[priority].Count == 0)
                data.Remove(priority);
        }

        public T Get(int priority, int index)
        {
            return data[priority][index];
        }
        object IPriorityList.Get(int priority, int index) { return Get(priority, index); }

        public void Set(int priority, int index, T item)
        {
            data[priority][index] = item;
        }
        void IPriorityList.Set(int priority, int index, object item) { Set(priority, index, (T)item); }

        public void Clear()
        {
            data.Clear();
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
        IEnumerable IPriorityList.GetItems(int priority) { return GetItems(priority); }


        public IEnumerator<T> GetEnumerator()
        {
            foreach (int key in data.Keys)
            {
                foreach (T item in data[key])
                    yield return item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

        public int GetCountAtPriority(int priority)
        {
            List<T> items;
            if (data.TryGetValue(priority, out items))
                return items.Count;
            return 0;
        }
        int IPriorityList.GetCountAtPriority(int priority) { return GetCountAtPriority(priority); }

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

