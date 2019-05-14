// <copyright file="SpawnRangeList.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A data structure representing spawn rates of items spread across a range of floors.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SpawnRangeList<T>
    {
        [Serializable]
        private class SpawnRange
        {
            public T Spawn;
            public int Rate;
            public Range Range;

            public SpawnRange(T item, int rate, Range range)
            {
                Spawn = item;
                Rate = rate;
                Range = range;
            }
        }
        //TODO: Binary Space Partition Tree
        private readonly List<SpawnRange> spawns;

        public int Count { get { return spawns.Count; } }

        public SpawnRangeList()
        {
            spawns = new List<SpawnRange>();
        }

        public void Add(T spawn, Range range)
        {
            Add(spawn, range, 10);
        }

        public void Add(T spawn, Range range, int rate)
        {
            if (rate < 0)
                throw new ArgumentException("Spawn rate must be 0 or higher.");
            if (range.Length <= 0)
                throw new ArgumentException("Spawn range must be 1 or higher.");
            spawns.Add(new SpawnRange(spawn, rate, range));
        }

        public void Remove(T spawn)
        {
            for (int ii = 0; ii < spawns.Count; ii++)
            {
                if (spawns[ii].Spawn.Equals(spawn))
                {
                    spawns.RemoveAt(ii);
                    return;
                }
            }
            throw new InvalidOperationException("Cannot find spawn!");
        }

        public void Clear()
        {
            spawns.Clear();
        }

        public IEnumerable<T> GetSpawns()
        {
            foreach (SpawnRange spawn in spawns)
                yield return spawn.Spawn;
        }

        private IEnumerable<SpawnRange> getSpawns(int level)
        {
            foreach (SpawnRange spawn in spawns)
            {
                if (spawn.Range.Min <= level && level < spawn.Range.Max)
                    yield return spawn;
            }
        }
        public SpawnList<T> GetSpawnList(int level)
        {
            SpawnList<T> newList = new SpawnList<T>();
            foreach (SpawnRange spawn in spawns)
            {
                if (spawn.Range.Min <= level && level < spawn.Range.Max)
                    newList.Add(spawn.Spawn, spawn.Rate);
            }
            return newList;
        }


        public bool CanPick(int level)
        {
            foreach (SpawnRange spawn in spawns)
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
            foreach (SpawnRange spawn in getSpawns(level))
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



    }
}
