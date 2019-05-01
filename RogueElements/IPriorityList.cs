using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{

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

}

