// <copyright file="SpawnRangeList.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A data structure representing spawn rates of items spread across a range of floors.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    // TODO: Binary Space Partition Tree
    [Serializable]
    public class SpawnRangeList<T> : ISpawnRangeList<T>, ICollection<SpawnRangeList<T>.SpawnRange>, ISpawnRangeList
    {
        private readonly List<SpawnRange> spawns;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnRangeList{T}"/> class.
        /// </summary>
        public SpawnRangeList()
        {
            this.spawns = new List<SpawnRange>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnRangeList{T}"/> class by copying another instance.
        /// </summary>
        /// <param name="other">The instance to copy.</param>
        public SpawnRangeList(SpawnRangeList<T> other)
        {
            this.spawns = new List<SpawnRange>();
            foreach (SpawnRange item in other.spawns)
                this.spawns.Add(new SpawnRange(item.Spawn, item.Rate, item.Range));
        }

        /// <inheritdoc/>
        public int Count => this.spawns.Count;

        /// <inheritdoc/>
        bool ICollection<SpawnRange>.IsReadOnly => false;

        /// <summary>
        /// Creates a shallow copy of this instance.
        /// </summary>
        /// <returns>A new instance with the same state.</returns>
        public SpawnRangeList<T> CopyState() => new SpawnRangeList<T>(this);

        /// <inheritdoc/>
        void ICollection<SpawnRange>.Add(SpawnRange range)
        {
            if (range.Rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            if (range.Range.Length <= 0)
                throw new ArgumentException("Spawn range must be 1 or higher.");
            this.spawns.Add(range);
        }

        /// <inheritdoc/>
        public void Add(T spawn, IntRange range, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            if (range.Length <= 0)
                throw new ArgumentException("Spawn range must be 1 or higher.");
            this.spawns.Add(new SpawnRange(spawn, rate, range));
        }

        /// <inheritdoc/>
        public void Insert(int index, T spawn, IntRange range, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawns.Insert(index, new SpawnRange(spawn, rate, range));
        }

        /// <inheritdoc/>
        bool ICollection<SpawnRange>.Remove(SpawnRange randRange)
        {
            return this.spawns.Remove(randRange);
        }

        /// <summary>
        /// Removes the first occurrence of the specified item.
        /// </summary>
        /// <param name="spawn">The item to remove.</param>
        /// <exception cref="InvalidOperationException">Thrown when the item is not found.</exception>
        public void Remove(T spawn)
        {
            for (int ii = 0; ii < this.spawns.Count; ii++)
            {
                if (this.spawns[ii].Spawn.Equals(spawn))
                {
                    this.spawns.RemoveAt(ii);
                    return;
                }
            }

            throw new InvalidOperationException("Cannot find spawn!");
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.spawns.Clear();
        }

        /// <inheritdoc/>
        void ICollection<SpawnRange>.CopyTo(SpawnRange[] array, int arrayIndex)
        {
            foreach (SpawnRange spawn in this.spawns)
            {
                array[arrayIndex] = spawn;
                arrayIndex++;
            }
        }

        /// <summary>
        /// Enumerates all possible spawn outcomes.
        /// </summary>
        /// <returns>An enumerable of all items in the list.</returns>
        public IEnumerable<T> EnumerateOutcomes()
        {
            foreach (SpawnRange spawn in this.spawns)
                yield return spawn.Spawn;
        }

        /// <inheritdoc/>
        IEnumerator<SpawnRange> IEnumerable<SpawnRange>.GetEnumerator()
        {
            foreach (SpawnRange spawn in this.spawns)
                yield return spawn;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (SpawnRange spawn in this.spawns)
                yield return spawn;
        }

        /// <summary>
        /// Creates a spawn list containing only items valid for the specified level.
        /// </summary>
        /// <param name="level">The level to filter by.</param>
        /// <returns>A spawn list with items applicable to the level.</returns>
        public SpawnList<T> GetSpawnList(int level)
        {
            SpawnList<T> newList = new SpawnList<T>();
            foreach (SpawnRange spawn in this.spawns)
            {
                if (spawn.Range.Min <= level && level < spawn.Range.Max)
                    newList.Add(spawn.Spawn, spawn.Rate);
            }

            return newList;
        }

        /// <summary>
        /// Determines whether any item can be picked at the specified level.
        /// </summary>
        /// <param name="level">The level to check.</param>
        /// <returns>True if at least one item can spawn at this level.</returns>
        public bool CanPick(int level)
        {
            foreach (SpawnRange spawn in this.spawns)
            {
                if (spawn.Range.Min <= level && level < spawn.Range.Max && spawn.Rate > 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Picks a random item valid for the specified level.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <param name="level">The level to pick for.</param>
        /// <returns>A randomly selected item.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no items can spawn at this level.</exception>
        public T Pick(IRandom random, int level)
        {
            int spawnTotal = 0;
            List<SpawnRange> spawns = new List<SpawnRange>();
            foreach (SpawnRange spawn in this.GetLevelSpawns(level))
            {
                spawns.Add(spawn);
                spawnTotal += spawn.Rate;
            }

            if (spawnTotal > 0)
            {
                int rand = random.Next(spawnTotal);
                int total = 0;
                for (int ii = 0; ii < spawns.Count; ii++)
                {
                    total += spawns[ii].Rate;
                    if (rand < total)
                        return spawns[ii].Spawn;
                }
            }

            throw new InvalidOperationException("Cannot spawn from a spawnlist of total rate 0!");
        }

        /// <inheritdoc/>
        public T GetSpawn(int index)
        {
            return this.spawns[index].Spawn;
        }

        /// <summary>
        /// Gets the spawn rate of a specific item.
        /// </summary>
        /// <param name="spawn">The item to find.</param>
        /// <returns>The spawn rate of the item, or 0 if not found.</returns>
        public int GetSpawnRate(T spawn)
        {
            for (int ii = 0; ii < this.spawns.Count; ii++)
            {
                if (this.spawns[ii].Spawn.Equals(spawn))
                    return this.spawns[ii].Rate;
            }

            return 0;
        }

        /// <inheritdoc/>
        public int GetSpawnRate(int index)
        {
            return this.spawns[index].Rate;
        }

        /// <inheritdoc/>
        public IntRange GetSpawnRange(int index)
        {
            return this.spawns[index].Range;
        }

        /// <inheritdoc/>
        public void SetSpawn(int index, T spawn)
        {
            this.spawns[index] = new SpawnRange(spawn, this.spawns[index].Rate, this.spawns[index].Range);
        }

        /// <inheritdoc/>
        public void SetSpawnRate(int index, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawns[index] = new SpawnRange(this.spawns[index].Spawn, rate, this.spawns[index].Range);
        }

        /// <inheritdoc/>
        public void SetSpawnRange(int index, IntRange range)
        {
            this.spawns[index] = new SpawnRange(this.spawns[index].Spawn, this.spawns[index].Rate, range);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            this.spawns.RemoveAt(index);
        }

        /// <inheritdoc/>
        void ISpawnRangeList.Add(object spawn, IntRange range, int rate)
        {
            this.Add((T)spawn, range, rate);
        }

        /// <inheritdoc/>
        void ISpawnRangeList.Insert(int index, object spawn, IntRange range, int rate)
        {
            this.Insert(index, (T)spawn, range, rate);
        }

        /// <inheritdoc/>
        bool ICollection<SpawnRange>.Contains(SpawnRange item)
        {
            return this.spawns.Contains(item);
        }

        /// <inheritdoc/>
        object ISpawnRangeList.GetSpawn(int index)
        {
            return this.GetSpawn(index);
        }

        /// <inheritdoc/>
        void ISpawnRangeList.SetSpawn(int index, object spawn)
        {
            this.SetSpawn(index, (T)spawn);
        }

        private IEnumerable<SpawnRange> GetLevelSpawns(int level)
        {
            foreach (SpawnRange spawn in this.spawns)
            {
                if (spawn.Range.Min <= level && level < spawn.Range.Max)
                    yield return spawn;
            }
        }

        /// <summary>
        /// Represents an item with its spawn rate weight and level range.
        /// </summary>
        [Serializable]
        public struct SpawnRange
        {
            /// <summary>
            /// The spawnable item.
            /// </summary>
            public T Spawn;

            /// <summary>
            /// The spawn rate weight for this item.
            /// </summary>
            public int Rate;

            /// <summary>
            /// The level range where this item can spawn.
            /// </summary>
            public IntRange Range;

            /// <summary>
            /// Initializes a new instance of the <see cref="SpawnRange"/> struct.
            /// </summary>
            /// <param name="item">The spawnable item.</param>
            /// <param name="rate">The spawn rate weight.</param>
            /// <param name="range">The level range constraint.</param>
            public SpawnRange(T item, int rate, IntRange range)
            {
                this.Spawn = item;
                this.Rate = rate;
                this.Range = range;
            }
        }
    }
}
