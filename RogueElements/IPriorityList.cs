// <copyright file="IPriorityList.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RogueElements
{
    [SuppressMessage(
        "Microsoft.Design",
        "CA1010:CollectionsShouldImplementGenericInterface",
        MessageId = nameof(IPriorityList),
        Justification = "Non-generic interface for typically generic classes")]
    public interface IPriorityList : IEnumerable
    {
        int PriorityCount { get; }

        int Count { get; }

        void Add(int priority, object item);

        void Insert(int priority, int index, object item);

        void RemoveAt(int priority, int index);

        object Get(int priority, int index);

        void Set(int priority, int index, object item);

        void Clear();

        int GetCountAtPriority(int priority);

        IEnumerable<int> GetPriorities();

        IEnumerable GetItems(int priority);
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
