// <copyright file="ISpawnList.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    public interface ISpawnList<T> : IEnumerable
    {
        int Count { get; }

        int SpawnTotal { get; }

        void Insert(int index, T spawn, int rate);

        void Add(T spawn, int rate);

        void Clear();

        T GetSpawn(int index);

        int GetSpawnRate(int index);

        void SetSpawn(int index, T spawn);

        void SetSpawnRate(int index, int rate);

        void RemoveAt(int index);
    }

    public interface ISpawnList : IEnumerable
    {
        int Count { get; }

        int SpawnTotal { get; }

        void Insert(int index, object spawn, int rate);

        void Add(object spawn, int rate);

        void Clear();

        object GetSpawn(int index);

        int GetSpawnRate(int index);

        void SetSpawn(int index, object spawn);

        void SetSpawnRate(int index, int rate);

        void RemoveAt(int index);
    }
}
