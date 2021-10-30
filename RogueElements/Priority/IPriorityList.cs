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
    public interface IPriorityList
    {
        int PriorityCount { get; }

        int Count { get; }

        void Add(Priority priority, object item);

        void Insert(Priority priority, int index, object item);

        void RemoveAt(Priority priority, int index);

        object Get(Priority priority, int index);

        void Set(Priority priority, int index, object item);

        void Clear();

        int GetCountAtPriority(Priority priority);

        IEnumerable<Priority> GetPriorities();

        IEnumerable GetItems(Priority priority);
    }

    public interface IPriorityList<T> : IPriorityList
    {
        void Add(Priority priority, T item);

        void Insert(Priority priority, int index, T item);

        new T Get(Priority priority, int index);

        void Set(Priority priority, int index, T item);

        new IEnumerable<T> GetItems(Priority priority);
    }
}
