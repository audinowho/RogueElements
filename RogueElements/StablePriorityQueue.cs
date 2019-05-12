// <copyright file="StablePriorityQueue.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    //MIN queue
    public class StablePriorityQueue<P, T> where P : IComparable<P>
    {
        private class StablePriorityQueueItem<IP, IT> where IP : IComparable<P>
        {
            public IP priority;
            public uint insertOrder;
            public IT value;

            public StablePriorityQueueItem(IP p, uint i, IT v)
            {
                priority = p;
                insertOrder = i;
                value = v;
            }
        }

        //uses Heap, represented as list
        private List<StablePriorityQueueItem<P, T>> data;
        private uint insertions;

        public StablePriorityQueue()
        {
            this.data = new List<StablePriorityQueueItem<P, T>>();
        }

        private bool isLarger(int index1, int index2)
        {
            return (data[index1].priority.CompareTo(data[index2].priority) > 0 || (data[index1].priority.CompareTo(data[index2].priority) == 0 && data[index1].insertOrder > data[index2].insertOrder));
        }

        public void Enqueue(P priority, T item)
        {
            data.Add(new StablePriorityQueueItem<P, T>(priority, insertions, item));
            insertions++;

            int child = data.Count - 1; // child index; start at end
            while (child > 0)
            {
                int parent = (child - 1) / 2; // parent index
                if (isLarger(child, parent))
                    break; // child item is larger than (or equal) parent so we're done
                StablePriorityQueueItem<P, T> tmp = data[child];
                data[child] = data[parent];
                data[parent] = tmp;
                child = parent;
            }
        }

        public delegate void PriorityOp<N>(N inPriority);

        public void OperateAllPriority(PriorityOp<P> op)
        {
            foreach (StablePriorityQueueItem<P, T> item in data)
                op(item.priority);
        }

        public void AddOrSetPriority(P priority, T item)
        {
            StablePriorityQueueItem<P, T> found_item = null;
            //search for item
            for (int ii = 0; ii < data.Count; ii++)
            {
                if (data[ii].value.Equals(item))
                {
                    //if item is found, get the tuple
                    found_item = data[ii];

                    //then delete
                    deleteAt(ii);
                    break;
                }
            }

            //re-add item
            if (found_item == null)
            {
                found_item = new StablePriorityQueueItem<P, T>(priority, insertions, item);
                insertions++;
            }
            data.Add(new StablePriorityQueueItem<P, T>(priority, insertions, item));
            int child = data.Count - 1; // child index; start at end
            while (child > 0)
            {
                int parent = (child - 1) / 2; // parent index
                if (isLarger(child, parent))
                    break; // child item is larger than (or equal) parent so we're done
                StablePriorityQueueItem<P, T> tmp = data[child];
                data[child] = data[parent];
                data[parent] = tmp;
                child = parent;
            }
        }

        public T Dequeue()
        {
            // assumes pq is not empty; up to calling code
            StablePriorityQueueItem<P, T> frontItem = data[0];   // fetch the front
            deleteAt(0);

            return frontItem.value;
        }

        public T Front()
        {
            // assumes pq is not empty; up to calling code
            StablePriorityQueueItem<P, T> frontItem = data[0];   // fetch the front

            return frontItem.value;
        }

        public P FrontPriority()
        {
            // assumes pq is not empty; up to calling code
            StablePriorityQueueItem<P, T> frontItem = data[0];   // fetch the front

            return frontItem.priority;
        }

        public bool TryGetPriority(T item, out P priority)
        {
            for (int ii = 0; ii < data.Count; ii++)
            {
                if (data[ii].value == null)
                {
                    if (item == null)
                    {
                        priority = data[ii].priority;
                        return true;
                    }
                }
                else if (data[ii].value.Equals(item))
                {
                    priority = data[ii].priority;
                    return true;
                }
            }

            priority = default(P);
            return false;
        }

        public void RemoveItem(T item)
        {
            for (int ii = 0; ii < data.Count; ii++)
            {
                if (data[ii].value == null)
                {
                    if (item == null)
                    {
                        deleteAt(ii);
                        return;
                    }
                }
                else if (data[ii].value.Equals(item))
                {
                    deleteAt(ii);
                    return;
                }
            }
        }

        private void deleteAt(int index)
        {
            int lastIndex = data.Count - 1; // last index (before removal)
            data[index] = data[lastIndex];
            data.RemoveAt(lastIndex);

            lastIndex--; // last index (after removal)
            int parent = index; // parent index. start at front of pq
            while (true)
            {
                int child = parent * 2 + 1; // left child index of parent
                if (child > lastIndex)
                    break;  // no children so done
                int rc = child + 1;     // right child
                if (rc <= lastIndex && !isLarger(rc, child)) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                    child = rc;
                if (!isLarger(parent, child))
                    break; // parent is smaller than (or equal to) smallest child so done
                StablePriorityQueueItem<P, T> tmp = data[parent];
                data[parent] = data[child];
                data[child] = tmp; // swap parent and child
                parent = child;
            }

        }

        public void Clear()
        {
            data.Clear();
            insertions = 0;
        }

        public int Count { get { return data.Count; } }
    }

}

