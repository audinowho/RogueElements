// <copyright file="StablePriorityQueue.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A min-heap priority queue that maintains insertion order for equal priorities.
    /// </summary>
    /// <typeparam name="TPriority">The type of priority values.</typeparam>
    /// <typeparam name="TValue">The type of values stored.</typeparam>
    public class StablePriorityQueue<TPriority, TValue>
        where TPriority : IComparable<TPriority>
    {
        private readonly List<StablePriorityQueueItem<TPriority, TValue>> data;
        private uint insertions;

        /// <summary>
        /// Initializes a new instance of the <see cref="StablePriorityQueue{TPriority, TValue}"/> class.
        /// </summary>
        public StablePriorityQueue()
        {
            this.data = new List<StablePriorityQueueItem<TPriority, TValue>>();
        }

        /// <summary>
        /// Delegate for operating on priority values.
        /// </summary>
        /// <typeparam name="T">The priority type.</typeparam>
        /// <param name="inPriority">The priority value.</param>
        public delegate void PriorityOp<T>(T inPriority);

        /// <summary>
        /// Gets the number of items in the queue.
        /// </summary>
        public int Count => this.data.Count;

        /// <summary>
        /// Adds an item with the specified priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="item">The item to add.</param>
        public void Enqueue(TPriority priority, TValue item)
        {
            this.data.Add(new StablePriorityQueueItem<TPriority, TValue>(priority, this.insertions, item));
            this.insertions++;

            int child = this.data.Count - 1; // child index; start at end
            while (child > 0)
            {
                int parent = (child - 1) / 2; // parent index
                if (this.IsLarger(child, parent))
                    break; // child item is larger than (or equal) parent so we're done
                StablePriorityQueueItem<TPriority, TValue> tmp = this.data[child];
                this.data[child] = this.data[parent];
                this.data[parent] = tmp;
                child = parent;
            }
        }

        /// <summary>
        /// Applies an operation to all priorities in the queue.
        /// </summary>
        /// <param name="op">The operation to apply.</param>
        public void OperateAllPriority(PriorityOp<TPriority> op)
        {
            if (op == null)
                throw new ArgumentNullException(nameof(op));

            foreach (StablePriorityQueueItem<TPriority, TValue> item in this.data)
                op(item.Priority);
        }

        /// <summary>
        /// Adds an item or updates its priority if it already exists.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="item">The item to add or update.</param>
        public void AddOrSetPriority(TPriority priority, TValue item)
        {
            bool isInQueue = false;

            // search for item
            for (int ii = 0; ii < this.data.Count; ii++)
            {
                if (this.data[ii].Value.Equals(item))
                {
                    // if item is found, delete it
                    isInQueue = true;
                    this.DeleteAt(ii);
                    break;
                }
            }

            // re-add item
            if (!isInQueue)
                this.insertions++;

            this.data.Add(new StablePriorityQueueItem<TPriority, TValue>(priority, this.insertions, item));
            int child = this.data.Count - 1; // child index; start at end
            while (child > 0)
            {
                int parent = (child - 1) / 2; // parent index
                if (this.IsLarger(child, parent))
                    break; // child item is larger than (or equal) parent so we're done
                StablePriorityQueueItem<TPriority, TValue> tmp = this.data[child];
                this.data[child] = this.data[parent];
                this.data[parent] = tmp;
                child = parent;
            }
        }

        /// <summary>
        /// Removes and returns the item with the lowest priority.
        /// </summary>
        /// <returns>The dequeued item.</returns>
        public TValue Dequeue()
        {
            StablePriorityQueueItem<TPriority, TValue> frontItem = this.data[0];
            this.DeleteAt(0);

            return frontItem.Value;
        }

        /// <summary>
        /// Returns the item with the lowest priority without removing it.
        /// </summary>
        /// <returns>The front item.</returns>
        public TValue Front()
        {
            // assumes pq is not empty; up to calling code
            StablePriorityQueueItem<TPriority, TValue> frontItem = this.data[0];   // fetch the front

            return frontItem.Value;
        }

        /// <summary>
        /// Returns the priority of the front item without removing it.
        /// </summary>
        /// <returns>The front priority.</returns>
        public TPriority FrontPriority()
        {
            StablePriorityQueueItem<TPriority, TValue> frontItem = this.data[0];

            return frontItem.Priority;
        }

        /// <summary>
        /// Attempts to get the priority of an item in the queue.
        /// </summary>
        /// <param name="item">The item to find.</param>
        /// <param name="priority">The priority if found.</param>
        /// <returns><c>true</c> if the item was found; otherwise <c>false</c>.</returns>
        public bool TryGetPriority(TValue item, out TPriority priority)
        {
            for (int ii = 0; ii < this.data.Count; ii++)
            {
                if (this.data[ii].Value == null)
                {
                    if (item == null)
                    {
                        priority = this.data[ii].Priority;
                        return true;
                    }
                }
                else if (this.data[ii].Value.Equals(item))
                {
                    priority = this.data[ii].Priority;
                    return true;
                }
            }

            priority = default;
            return false;
        }

        /// <summary>
        /// Removes an item from the queue.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(TValue item)
        {
            for (int ii = this.data.Count - 1; ii >= 0; ii--)
            {
                if (this.data[ii].Value == null)
                {
                    if (item == null)
                    {
                        this.DeleteAt(ii);
                        return;
                    }
                }
                else if (this.data[ii].Value.Equals(item))
                {
                    this.DeleteAt(ii);
                    return;
                }
            }
        }

        /// <summary>
        /// Removes all items from the queue.
        /// </summary>
        public void Clear()
        {
            this.data.Clear();
            this.insertions = 0;
        }

        private bool IsLarger(int index1, int index2)
        {
            var item1 = this.data[index1];
            var item2 = this.data[index2];
            return item1.Priority.CompareTo(item2.Priority) > 0 || (item1.Priority.CompareTo(item2.Priority) == 0 && item1.InsertOrder > item2.InsertOrder);
        }

        private void DeleteAt(int index)
        {
            int lastIndex = this.data.Count - 1; // last index (before removal)
            this.data[index] = this.data[lastIndex];
            this.data.RemoveAt(lastIndex);

            lastIndex--; // last index (after removal)
            int parent = index; // parent index. start at front of pq
            while (true)
            {
                int child = (parent * 2) + 1; // left child index of parent
                if (child > lastIndex)
                    break;  // no children so done
                int rc = child + 1;     // right child
                if (rc <= lastIndex && !this.IsLarger(rc, child)) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                    child = rc;
                if (!this.IsLarger(parent, child))
                    break; // parent is smaller than (or equal to) smallest child so done
                StablePriorityQueueItem<TPriority, TValue> tmp = this.data[parent];
                this.data[parent] = this.data[child];
                this.data[child] = tmp; // swap parent and child
                parent = child;
            }
        }

        private struct StablePriorityQueueItem<TItemPriority, TItemValue>
            where TItemPriority : IComparable<TPriority>
        {
            public TItemPriority Priority;
            public uint InsertOrder;
            public TItemValue Value;

            public StablePriorityQueueItem(TItemPriority p, uint i, TItemValue v)
            {
                this.Priority = p;
                this.InsertOrder = i;
                this.Value = v;
            }
        }
    }
}
