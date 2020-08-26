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
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SpawnList<T> : IRandPicker<T>, ISpawnList<T>, ISpawnList
    {
        private readonly List<SpawnRate> spawns;
        private int spawnTotal;

        public SpawnList()
        {
            this.spawns = new List<SpawnRate>();
        }

        protected SpawnList(SpawnList<T> other)
        {
            this.spawns = new List<SpawnRate>(other.spawns);
            foreach (SpawnRate item in other.spawns)
                this.spawns.Add(new SpawnRate(item.Spawn, item.Rate));
        }

        public int Count => this.spawns.Count;

        public int SpawnTotal => this.spawnTotal;

        public bool CanPick => this.spawnTotal > 0;

        public bool ChangesState => false;

        public IRandPicker<T> CopyState() => new SpawnList<T>(this);

        public void Add(T spawn)
        {
            this.Add(spawn, 10);
        }

        public void Add(T spawn, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawns.Add(new SpawnRate(spawn, rate));
            this.spawnTotal += rate;
        }

        public void Insert(int index, T spawn, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawns.Insert(index, new SpawnRate(spawn, rate));
            this.spawnTotal += rate;
        }

        public void Clear()
        {
            this.spawns.Clear();
            this.spawnTotal = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (SpawnRate element in this.spawns)
                yield return element.Spawn;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public T Pick(IRandom random)
        {
            int ii = this.PickIndex(random);
            return this.spawns[ii].Spawn;
        }

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

        public void SetSpawn(int index, T spawn)
        {
            this.spawns[index] = new SpawnRate(spawn, this.spawns[index].Rate);
        }

        public void SetSpawnRate(int index, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            this.spawnTotal = this.spawnTotal - this.spawns[index].Rate + rate;
            this.spawns[index] = new SpawnRate(this.spawns[index].Spawn, rate);
        }

        public void RemoveAt(int index)
        {
            this.spawnTotal -= this.spawns[index].Rate;
            this.spawns.RemoveAt(index);
        }

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

        public override int GetHashCode()
        {
            int code = 0;
            for (int ii = 0; ii < this.spawns.Count; ii++)
                code ^= this.spawns[ii].Spawn.GetHashCode() ^ this.spawns[ii].Rate;
            return code;
        }

        void ISpawnList.Add(object spawn, int rate)
        {
            this.Add((T)spawn, rate);
        }

        void ISpawnList.Insert(int index, object spawn, int rate)
        {
            this.Insert(index, (T)spawn, rate);
        }

        object ISpawnList.GetSpawn(int index)
        {
            return this.GetSpawn(index);
        }

        void ISpawnList.SetSpawn(int index, object spawn)
        {
            this.SetSpawn(index, (T)spawn);
        }

        [Serializable]
        private struct SpawnRate
        {
            public T Spawn;
            public int Rate;

            public SpawnRate(T item, int rate)
            {
                this.Spawn = item;
                this.Rate = rate;
            }
        }
    }
}
