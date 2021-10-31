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
    /// <typeparam name="T"></typeparam>
    // TODO: Binary Space Partition Tree
    [Serializable]
    public class SpawnRangeList<T> : ISpawnRangeList<T>, ICollection<SpawnRangeList<T>.SpawnRange>, ISpawnRangeList
    {
        private readonly List<SpawnRange> spawns;

        public SpawnRangeList()
        {
            this.spawns = new List<SpawnRange>();
        }

        public SpawnRangeList(SpawnRangeList<T> other)
        {
            this.spawns = new List<SpawnRange>();
            foreach (SpawnRange item in other.spawns)
                this.spawns.Add(new SpawnRange(item.Spawn, item.Rate, item.Range));
        }

        public int Count => this.spawns.Count;

        bool ICollection<SpawnRange>.IsReadOnly => false;

        /// <summary>
        /// This is a shallow copy.
        /// </summary>
        /// <returns></returns>
        public SpawnRangeList<T> CopyState() => new SpawnRangeList<T>(this);

        void ICollection<SpawnRange>.Add(SpawnRange range)
        {
            if (range.Rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            if (range.Range.Length <= 0)
                throw new ArgumentException("Spawn range must be 1 or higher.");
            this.spawns.Add(range);
        }

        public void Add(T spawn, IntRange range, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            if (range.Length <= 0)
                throw new ArgumentException("Spawn range must be 1 or higher.");
            this.spawns.Add(new SpawnRange(spawn, rate, range));
        }

        public void Insert(int index, T spawn, IntRange range, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawns.Insert(index, new SpawnRange(spawn, rate, range));
        }

        bool ICollection<SpawnRange>.Remove(SpawnRange randRange)
        {
            return this.spawns.Remove(randRange);
        }

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

        public void Clear()
        {
            this.spawns.Clear();
        }

        void ICollection<SpawnRange>.CopyTo(SpawnRange[] array, int arrayIndex)
        {
            foreach (SpawnRange spawn in this.spawns)
            {
                array[arrayIndex] = spawn;
                arrayIndex++;
            }
        }

        public IEnumerable<T> EnumerateOutcomes()
        {
            foreach (SpawnRange spawn in this.spawns)
                yield return spawn.Spawn;
        }

        IEnumerator<SpawnRange> IEnumerable<SpawnRange>.GetEnumerator()
        {
            foreach (SpawnRange spawn in this.spawns)
                yield return spawn;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (SpawnRange spawn in this.spawns)
                yield return spawn;
        }

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

        public bool CanPick(int level)
        {
            foreach (SpawnRange spawn in this.spawns)
            {
                if (spawn.Range.Min <= level && level < spawn.Range.Max && spawn.Rate > 0)
                    return true;
            }

            return false;
        }

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

        public T GetSpawn(int index)
        {
            return this.spawns[index].Spawn;
        }

        public int GetSpawnRate(T spawn)
        {
            for (int ii = 0; ii < this.spawns.Count; ii++)
            {
                if (this.spawns[ii].Spawn.Equals(spawn))
                    return this.spawns[ii].Rate;
            }

            return 0;
        }

        public int GetSpawnRate(int index)
        {
            return this.spawns[index].Rate;
        }

        public IntRange GetSpawnRange(int index)
        {
            return this.spawns[index].Range;
        }

        public void SetSpawn(int index, T spawn)
        {
            this.spawns[index] = new SpawnRange(spawn, this.spawns[index].Rate, this.spawns[index].Range);
        }

        public void SetSpawnRate(int index, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawns[index] = new SpawnRange(this.spawns[index].Spawn, rate, this.spawns[index].Range);
        }

        public void SetSpawnRange(int index, IntRange range)
        {
            this.spawns[index] = new SpawnRange(this.spawns[index].Spawn, this.spawns[index].Rate, range);
        }

        public void RemoveAt(int index)
        {
            this.spawns.RemoveAt(index);
        }

        void ISpawnRangeList.Add(object spawn, IntRange range, int rate)
        {
            this.Add((T)spawn, range, rate);
        }

        void ISpawnRangeList.Insert(int index, object spawn, IntRange range, int rate)
        {
            this.Insert(index, (T)spawn, range, rate);
        }

        bool ICollection<SpawnRange>.Contains(SpawnRange item)
        {
            return this.spawns.Contains(item);
        }

        object ISpawnRangeList.GetSpawn(int index)
        {
            return this.GetSpawn(index);
        }

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

        [Serializable]
        public struct SpawnRange
        {
            public T Spawn;
            public int Rate;
            public IntRange Range;

            public SpawnRange(T item, int rate, IntRange range)
            {
                this.Spawn = item;
                this.Rate = rate;
                this.Range = range;
            }
        }
    }
}
