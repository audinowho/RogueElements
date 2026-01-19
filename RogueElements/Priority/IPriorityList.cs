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
    /// <summary>
    /// Defines the non-generic contract for a priority-ordered collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="IPriorityList"/> provides a type-agnostic interface for priority-ordered collections,
    /// enabling reflection-based access and serialization scenarios where the item type is not known
    /// at compile time.
    /// </para>
    /// <para>
    /// For typed access, use the generic <see cref="IPriorityList{T}"/> interface or the
    /// <see cref="PriorityList{T}"/> implementation directly.
    /// </para>
    /// </remarks>
    /// <seealso cref="IPriorityList{T}"/>
    /// <seealso cref="PriorityList{T}"/>
    [SuppressMessage(
        "Microsoft.Design",
        "CA1010:CollectionsShouldImplementGenericInterface",
        MessageId = nameof(IPriorityList),
        Justification = "Non-generic interface for typically generic classes")]
    public interface IPriorityList
    {
        /// <summary>
        /// Gets the number of distinct priority levels that contain items.
        /// </summary>
        int PriorityCount { get; }

        /// <summary>
        /// Gets the total number of items across all priority levels.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds an item at the specified priority level.
        /// </summary>
        /// <param name="priority">The priority at which to add the item.</param>
        /// <param name="item">The item to add.</param>
        void Add(Priority priority, object item);

        /// <summary>
        /// Inserts an item at a specific index within a priority level.
        /// </summary>
        /// <param name="priority">The priority at which to insert the item.</param>
        /// <param name="index">The zero-based index at which to insert the item within that priority.</param>
        /// <param name="item">The item to insert.</param>
        void Insert(Priority priority, int index, object item);

        /// <summary>
        /// Removes the item at the specified index within a priority level.
        /// </summary>
        /// <param name="priority">The priority level containing the item to remove.</param>
        /// <param name="index">The zero-based index of the item to remove within that priority.</param>
        void RemoveAt(Priority priority, int index);

        /// <summary>
        /// Gets the item at the specified index within a priority level.
        /// </summary>
        /// <param name="priority">The priority level to access.</param>
        /// <param name="index">The zero-based index of the item within that priority.</param>
        /// <returns>The item at the specified position.</returns>
        object Get(Priority priority, int index);

        /// <summary>
        /// Sets the item at the specified index within a priority level.
        /// </summary>
        /// <param name="priority">The priority level to modify.</param>
        /// <param name="index">The zero-based index of the item to replace.</param>
        /// <param name="item">The new item value.</param>
        void Set(Priority priority, int index, object item);

        /// <summary>
        /// Removes all items from the priority list.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the number of items at a specific priority level.
        /// </summary>
        /// <param name="priority">The priority level to count items at.</param>
        /// <returns>The number of items at the specified priority, or 0 if the priority does not exist.</returns>
        int GetCountAtPriority(Priority priority);

        /// <summary>
        /// Enumerates all priority levels that contain items, in ascending order.
        /// </summary>
        /// <returns>An enumerable of all <see cref="Priority"/> values with at least one item.</returns>
        IEnumerable<Priority> GetPriorities();

        /// <summary>
        /// Gets all items at the specified priority level.
        /// </summary>
        /// <param name="priority">The priority level to retrieve items from.</param>
        /// <returns>An enumerable of items at the specified priority.</returns>
        IEnumerable GetItems(Priority priority);
    }

    /// <summary>
    /// Defines the generic contract for a priority-ordered collection.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the collection.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="IPriorityList{T}"/> extends <see cref="IPriorityList"/> with strongly-typed
    /// methods for adding, retrieving, and modifying items by priority.
    /// </para>
    /// <para>
    /// The primary implementation is <see cref="PriorityList{T}"/>, which is used by
    /// <see cref="MapGen{T}"/> to store <see cref="GenStep{T}"/> instances.
    /// </para>
    /// </remarks>
    /// <seealso cref="IPriorityList"/>
    /// <seealso cref="PriorityList{T}"/>
    public interface IPriorityList<T> : IPriorityList
    {
        /// <summary>
        /// Adds an item at the specified priority level.
        /// </summary>
        /// <param name="priority">The priority at which to add the item.</param>
        /// <param name="item">The item to add.</param>
        void Add(Priority priority, T item);

        /// <summary>
        /// Inserts an item at a specific index within a priority level.
        /// </summary>
        /// <param name="priority">The priority at which to insert the item.</param>
        /// <param name="index">The zero-based index at which to insert the item within that priority.</param>
        /// <param name="item">The item to insert.</param>
        void Insert(Priority priority, int index, T item);

        /// <summary>
        /// Gets the item at the specified index within a priority level.
        /// </summary>
        /// <param name="priority">The priority level to access.</param>
        /// <param name="index">The zero-based index of the item within that priority.</param>
        /// <returns>The item at the specified position.</returns>
        new T Get(Priority priority, int index);

        /// <summary>
        /// Sets the item at the specified index within a priority level.
        /// </summary>
        /// <param name="priority">The priority level to modify.</param>
        /// <param name="index">The zero-based index of the item to replace.</param>
        /// <param name="item">The new item value.</param>
        void Set(Priority priority, int index, T item);

        /// <summary>
        /// Gets all items at the specified priority level.
        /// </summary>
        /// <param name="priority">The priority level to retrieve items from.</param>
        /// <returns>An enumerable of items at the specified priority, in insertion order.</returns>
        new IEnumerable<T> GetItems(Priority priority);
    }
}
