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
    /// Stores and retrieves items organized by <see cref="Priority"/>, providing ordered access for generation pipelines.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the list.</typeparam>
    /// <remarks>
    /// <para>
    /// <see cref="PriorityList{T}"/> is the primary container used by <see cref="MapGen{T}"/> to store
    /// <see cref="GenStep{T}"/> instances. It allows multiple items at the same priority while maintaining
    /// consistent ordering during enumeration.
    /// </para>
    /// <para>
    /// Key features:
    /// <list type="bullet">
    /// <item><description>Items are grouped by <see cref="Priority"/> and enumerated in priority order</description></item>
    /// <item><description>Multiple items can share the same priority and are returned in insertion order</description></item>
    /// <item><description>Supports both simple integer priorities and hierarchical <see cref="Priority"/> values</description></item>
    /// <item><description>Efficient priority-based access and modification</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// The class implements both <see cref="IPriorityList{T}"/> for typed access and
    /// <see cref="ICollection{T}"/> for LINQ compatibility.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var steps = new PriorityList&lt;GenStep&lt;MyContext&gt;&gt;();
    ///
    /// // Add items with simple integer priorities
    /// steps.Add(1, new InitTilesStep());
    /// steps.Add(5, new PlaceRoomsStep());
    /// steps.Add(10, new FinalizeStep());
    ///
    /// // Add items with hierarchical priorities (between existing priorities)
    /// steps.Add(new Priority(5, 1), new AddDoorsStep());  // After PlaceRoomsStep, before FinalizeStep
    ///
    /// // Enumerate in priority order
    /// foreach (var step in steps.EnumerateInOrder())
    /// {
    ///     step.Apply(context);
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="Priority"/>
    /// <seealso cref="MapGen{T}"/>
    /// <seealso cref="IPriorityList{T}"/>
    [Serializable]
    public class PriorityList<T> : IPriorityList<T>, ICollection<KeyValuePair<Priority, T>>
    {
        private readonly Dictionary<Priority, List<T>> dict;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriorityList{T}"/> class with no items.
        /// </summary>
        public PriorityList()
        {
            this.dict = new Dictionary<Priority, List<T>>();
        }

        /// <summary>
        /// Gets the number of distinct priority levels that contain items.
        /// </summary>
        /// <value>
        /// The count of unique <see cref="Priority"/> values with at least one item.
        /// </value>
        public int PriorityCount => this.dict.Count;

        /// <summary>
        /// Gets the total number of items across all priority levels.
        /// </summary>
        /// <value>
        /// The sum of items at all priority levels.
        /// </value>
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

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<Priority, T>>.IsReadOnly => false;

        /// <summary>
        /// Adds an item with a simple integer priority.
        /// </summary>
        /// <param name="priority">The integer priority value. Lower values execute earlier.</param>
        /// <param name="item">The item to add.</param>
        /// <remarks>
        /// This is a convenience overload that wraps the integer in a <see cref="Priority"/> struct.
        /// </remarks>
        public void Add(int priority, T item)
        {
            this.Add(new Priority(priority), item);
        }

        /// <summary>
        /// Adds an item at the specified priority level.
        /// </summary>
        /// <param name="priority">The priority at which to add the item.</param>
        /// <param name="item">The item to add.</param>
        /// <remarks>
        /// If other items already exist at this priority, the new item is added after them.
        /// Items at the same priority are returned in insertion order during enumeration.
        /// </remarks>
        public void Add(Priority priority, T item)
        {
            if (!this.dict.ContainsKey(priority))
                this.dict[priority] = new List<T>();
            this.dict[priority].Add(item);
        }

        /// <inheritdoc/>
        void ICollection<KeyValuePair<Priority, T>>.Add(KeyValuePair<Priority, T> item) => this.Add(item.Key, item.Value);

        /// <inheritdoc/>
        void IPriorityList.Add(Priority priority, object item) => this.Add(priority, (T)item);

        /// <summary>
        /// Inserts an item at a specific index within an integer priority level.
        /// </summary>
        /// <param name="priority">The integer priority value.</param>
        /// <param name="index">The zero-based index at which to insert the item within that priority.</param>
        /// <param name="item">The item to insert.</param>
        public void Insert(int priority, int index, T item)
        {
            this.Insert(new Priority(priority), index, item);
        }

        /// <summary>
        /// Inserts an item at a specific index within a priority level.
        /// </summary>
        /// <param name="priority">The priority at which to insert the item.</param>
        /// <param name="index">The zero-based index at which to insert the item within that priority.</param>
        /// <param name="item">The item to insert.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="index"/> is not zero for a new priority level,
        /// or when it exceeds the count of items at an existing priority.
        /// </exception>
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

        /// <inheritdoc/>
        void IPriorityList.Insert(Priority priority, int index, object item) => this.Insert(priority, index, (T)item);

        /// <summary>
        /// Removes the item at the specified index within a priority level.
        /// </summary>
        /// <param name="priority">The priority level containing the item to remove.</param>
        /// <param name="index">The zero-based index of the item to remove within that priority.</param>
        /// <remarks>
        /// If removing the item leaves the priority level empty, the priority is automatically removed.
        /// </remarks>
        public void RemoveAt(Priority priority, int index)
        {
            this.dict[priority].RemoveAt(index);
            if (this.dict[priority].Count == 0)
                this.dict.Remove(priority);
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<Priority, T>>.Remove(KeyValuePair<Priority, T> item)
        {
            List<T> val;
            if (this.dict.TryGetValue(item.Key, out val))
                return val.Remove(item.Value);
            return false;
        }

        /// <summary>
        /// Gets the item at the specified index within a priority level.
        /// </summary>
        /// <param name="priority">The priority level to access.</param>
        /// <param name="index">The zero-based index of the item within that priority.</param>
        /// <returns>The item at the specified position.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the priority level does not exist.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
        public T Get(Priority priority, int index)
        {
            return this.dict[priority][index];
        }

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<Priority, T>>.Contains(KeyValuePair<Priority, T> item)
        {
            List<T> val;
            if (this.dict.TryGetValue(item.Key, out val))
                return val.Contains(item.Value);
            return false;
        }

        /// <inheritdoc/>
        object IPriorityList.Get(Priority priority, int index) => this.Get(priority, index);

        /// <summary>
        /// Sets the item at the specified index within a priority level.
        /// </summary>
        /// <param name="priority">The priority level to modify.</param>
        /// <param name="index">The zero-based index of the item to replace.</param>
        /// <param name="item">The new item value.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the priority level does not exist.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is out of range.</exception>
        public void Set(Priority priority, int index, T item)
        {
            this.dict[priority][index] = item;
        }

        /// <inheritdoc/>
        void IPriorityList.Set(Priority priority, int index, object item) => this.Set(priority, index, (T)item);

        /// <summary>
        /// Removes all items from the priority list.
        /// </summary>
        public void Clear()
        {
            this.dict.Clear();
        }

        /// <summary>
        /// Enumerates all priority levels in ascending order.
        /// </summary>
        /// <returns>An enumerable of all <see cref="Priority"/> values that contain items, sorted in ascending order.</returns>
        /// <remarks>
        /// Use this method with <see cref="GetItems"/> to iterate through items by priority level.
        /// </remarks>
        public IEnumerable<Priority> GetPriorities()
        {
            List<Priority> priorities = new List<Priority>();
            foreach (Priority key in this.dict.Keys)
                priorities.Add(key);

            priorities.Sort();

            foreach (Priority key in priorities)
                yield return key;
        }

        /// <summary>
        /// Gets all items at the specified priority level.
        /// </summary>
        /// <param name="priority">The priority level to retrieve items from.</param>
        /// <returns>An enumerable of items at the specified priority, in insertion order.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the priority level does not exist.</exception>
        public IEnumerable<T> GetItems(Priority priority)
        {
            foreach (T item in this.dict[priority])
                yield return item;
        }

        /// <inheritdoc/>
        IEnumerable IPriorityList.GetItems(Priority priority) => this.GetItems(priority);

        /// <summary>
        /// Enumerates all items in priority order.
        /// </summary>
        /// <returns>
        /// An enumerable of all items, first sorted by priority (ascending), then by insertion order within each priority.
        /// </returns>
        /// <remarks>
        /// This is the primary method used by <see cref="MapGen{T}"/> to iterate through generation steps.
        /// </remarks>
        public IEnumerable<T> EnumerateInOrder()
        {
            foreach (Priority key in this.GetPriorities())
            {
                foreach (T item in this.dict[key])
                    yield return item;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.EnumerateKeyValuePairs();

        /// <summary>
        /// Gets the number of items at a specific priority level.
        /// </summary>
        /// <param name="priority">The priority level to count items at.</param>
        /// <returns>The number of items at the specified priority, or 0 if the priority does not exist.</returns>
        public int GetCountAtPriority(Priority priority)
        {
            if (this.dict.TryGetValue(priority, out List<T> items))
                return items.Count;
            return 0;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        IEnumerator<KeyValuePair<Priority, T>> IEnumerable<KeyValuePair<Priority, T>>.GetEnumerator()
        {
            return this.EnumerateKeyValuePairs();
        }

        /// <inheritdoc/>
        int IPriorityList.GetCountAtPriority(Priority priority) => this.GetCountAtPriority(priority);

        /// <summary>
        /// Enumerates all items as key-value pairs of priority and item.
        /// </summary>
        /// <returns>
        /// An enumerator of key-value pairs where the key is the <see cref="Priority"/>
        /// and the value is the item, ordered by priority.
        /// </returns>
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
