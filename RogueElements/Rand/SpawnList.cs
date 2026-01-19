// <copyright file="SpawnList.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Selects an item randomly from a weighted list.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    [Serializable]
    public class SpawnList<T> : IRandPicker<T>, ISpawnList<T>, ICollection<SpawnList<T>.SpawnRate>, ISpawnList
    {
        private readonly List<SpawnRate> spawns;
        private int spawnTotal;
        private bool removeOnRoll;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnList{T}"/> class.
        /// </summary>
        public SpawnList()
        {
            this.spawns = new List<SpawnRate>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnList{T}"/> class with optional removal on pick.
        /// </summary>
        /// <param name="remove">If true, items are removed after being picked.</param>
        public SpawnList(bool remove)
        {
            this.removeOnRoll = remove;
            this.spawns = new List<SpawnRate>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpawnList{T}"/> class by copying another instance.
        /// </summary>
        /// <param name="other">The instance to copy.</param>
        protected SpawnList(SpawnList<T> other)
        {
            this.removeOnRoll = other.removeOnRoll;
            this.spawnTotal = other.spawnTotal;
            this.spawns = new List<SpawnRate>();
            foreach (SpawnRate item in other.spawns)
                this.spawns.Add(new SpawnRate(item.Spawn, item.Rate));
        }

        /// <inheritdoc/>
        public int Count => this.spawns.Count;

        /// <inheritdoc/>
        bool ICollection<SpawnRate>.IsReadOnly => false;

        /// <inheritdoc/>
        public int SpawnTotal => this.spawnTotal;

        /// <inheritdoc/>
        public bool CanPick => this.spawnTotal > 0;

        /// <summary>
        /// False if this is a bag with replacement.  True if not.
        /// </summary>
        public bool RemoveOnRoll => this.removeOnRoll;

        /// <inheritdoc/>
        public bool ChangesState => this.RemoveOnRoll;

        /// <summary>
        /// Creates a shallow copy of this instance.
        /// </summary>
        /// <returns>A new instance with the same state.</returns>
        public IRandPicker<T> CopyState() => new SpawnList<T>(this);

        /// <inheritdoc/>
        void ICollection<SpawnRate>.Add(SpawnRate spawnRate)
        {
            if (spawnRate.Rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawns.Add(spawnRate);
            this.spawnTotal += spawnRate.Rate;
        }

        /// <inheritdoc/>
        bool ICollection<SpawnRate>.Contains(SpawnRate item)
        {
            return this.spawns.Contains(item);
        }

        /// <inheritdoc/>
        public void Add(T spawn, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawns.Add(new SpawnRate(spawn, rate));
            this.spawnTotal += rate;
        }

        /// <inheritdoc/>
        public void Insert(int index, T spawn, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawns.Insert(index, new SpawnRate(spawn, rate));
            this.spawnTotal += rate;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.spawns.Clear();
            this.spawnTotal = 0;
        }

        /// <inheritdoc/>
        public IEnumerable<T> EnumerateOutcomes()
        {
            foreach (SpawnRate element in this.spawns)
                yield return element.Spawn;
        }

        /// <inheritdoc/>
        public IEnumerator<SpawnRate> GetEnumerator()
        {
            foreach (SpawnRate element in this.spawns)
                yield return element;
        }

        /// <inheritdoc/>
        void ICollection<SpawnRate>.CopyTo(SpawnRate[] array, int arrayIndex)
        {
            foreach (SpawnRate element in this.spawns)
            {
                array[arrayIndex] = element;
                arrayIndex++;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <inheritdoc/>
        public T Pick(IRandom random)
        {
            int ii = this.PickIndex(random);

            T spawn = this.spawns[ii].Spawn;

            if (this.RemoveOnRoll)
                this.RemoveAt(ii);

            return spawn;
        }

        /// <summary>
        /// Picks a random index from the list based on spawn rates.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>The index of the selected item.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the spawn total is zero.</exception>
        public int PickIndex(IRandom random)
        {
            if (this.spawnTotal > 0)
            {
                int rand = random.Next(this.spawnTotal);
                int total = 0;
                for (int ii = 0; ii < this.spawns.Count; ii++)
                {
                    total += this.spawns[ii].Rate;
                    if (rand < total)
                        return ii;
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
        public void SetSpawn(int index, T spawn)
        {
            this.spawns[index] = new SpawnRate(spawn, this.spawns[index].Rate);
        }

        /// <inheritdoc/>
        public void SetSpawnRate(int index, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawnTotal = this.spawnTotal - this.spawns[index].Rate + rate;
            this.spawns[index] = new SpawnRate(this.spawns[index].Spawn, rate);
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            this.spawnTotal -= this.spawns[index].Rate;
            this.spawns.RemoveAt(index);
        }

        /// <inheritdoc/>
        bool ICollection<SpawnRate>.Remove(SpawnRate spawnRate)
        {
            return this.spawns.Remove(spawnRate);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is SpawnList<T> other))
                return false;
            if (this.spawns.Count != other.spawns.Count)
                return false;
            for (int ii = 0; ii < this.spawns.Count; ii++)
            {
                if (!this.spawns[ii].Spawn.Equals(other.spawns[ii].Spawn))
                    return false;
                if (this.spawns[ii].Rate != other.spawns[ii].Rate)
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int code = 0;
            for (int ii = 0; ii < this.spawns.Count; ii++)
                code ^= this.spawns[ii].Spawn.GetHashCode() ^ this.spawns[ii].Rate;
            return code;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), this.spawns.Count);
        }

        /// <inheritdoc/>
        void ISpawnList.Add(object spawn, int rate)
        {
            this.Add((T)spawn, rate);
        }

        /// <inheritdoc/>
        void ISpawnList.Insert(int index, object spawn, int rate)
        {
            this.Insert(index, (T)spawn, rate);
        }

        /// <inheritdoc/>
        object ISpawnList.GetSpawn(int index)
        {
            return this.GetSpawn(index);
        }

        /// <inheritdoc/>
        void ISpawnList.SetSpawn(int index, object spawn)
        {
            this.SetSpawn(index, (T)spawn);
        }

        /// <summary>
        /// Represents an item with its spawn rate weight.
        /// </summary>
        [Serializable]
        public struct SpawnRate
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
            /// Initializes a new instance of the <see cref="SpawnRate"/> struct.
            /// </summary>
            /// <param name="item">The spawnable item.</param>
            /// <param name="rate">The spawn rate weight.</param>
            public SpawnRate(T item, int rate)
            {
                this.Spawn = item;
                this.Rate = rate;
            }
        }
    }
}
