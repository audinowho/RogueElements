// <copyright file="ISpawnRangeList.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    public interface ISpawnRangeList<T>
    {
        int Count { get; }

        void Insert(int index, T spawn, IntRange range, int rate);

        void Add(T spawn, IntRange range, int rate);

        void Clear();

        T GetSpawn(int index);

        IntRange GetSpawnRange(int index);

        int GetSpawnRate(int index);

        void SetSpawn(int index, T spawn);

        void SetSpawnRange(int index, IntRange range);

        void SetSpawnRate(int index, int rate);

        void RemoveAt(int index);
    }

    public interface ISpawnRangeList
    {
        int Count { get; }

        void Insert(int index, object spawn, IntRange range, int rate);

        void Add(object spawn, IntRange range, int rate);

        void Clear();

        object GetSpawn(int index);

        IntRange GetSpawnRange(int index);

        int GetSpawnRate(int index);

        void SetSpawn(int index, object spawn);

        void SetSpawnRange(int index, IntRange range);

        void SetSpawnRate(int index, int rate);

        void RemoveAt(int index);
    }
}
