using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RogueElements
{
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface",
        MessageId = "IPriorityList", Justification = "Non-generic interface for typically generic classes")]
    public interface IPriorityList : IEnumerable
    {

        void Add(int priority, object item);

        void Insert(int priority, int index, object item);

        void RemoveAt(int priority, int index);

        object Get(int priority, int index);
        void Set(int priority, int index, object item);
        void Clear();

        int GetCountAtPriority(int priority);

        IEnumerable<int> GetPriorities();

        IEnumerable GetItems(int priority);


        int PriorityCount { get; }
        int Count { get; }
    }

    public interface IPriorityList<T> : IEnumerable<T>, IPriorityList
    {
        void Add(int priority, T item);

        void Insert(int priority, int index, T item);
        new T Get(int priority, int index);
        void Set(int priority, int index, T item);

        new IEnumerable<T> GetItems(int priority);
    }

}

