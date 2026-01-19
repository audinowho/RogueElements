// <copyright file="ISpawnRangeList.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a weighted list of spawnable items with level range constraints.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public interface ISpawnRangeList<T>
    {
        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Inserts an item at the specified index with a level range and spawn rate.
        /// </summary>
        /// <param name="index">The index at which to insert.</param>
        /// <param name="spawn">The item to insert.</param>
        /// <param name="range">The level range where the item can spawn.</param>
        /// <param name="rate">The spawn rate weight.</param>
        void Insert(int index, T spawn, IntRange range, int rate);

        /// <summary>
        /// Adds an item to the list with a level range and spawn rate.
        /// </summary>
        /// <param name="spawn">The item to add.</param>
        /// <param name="range">The level range where the item can spawn.</param>
        /// <param name="rate">The spawn rate weight.</param>
        void Add(T spawn, IntRange range, int rate);

        /// <summary>
        /// Removes all items from the list.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item at the index.</returns>
        T GetSpawn(int index);

        /// <summary>
        /// Gets the level range of the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The level range constraint.</returns>
        IntRange GetSpawnRange(int index);

        /// <summary>
        /// Gets the spawn rate of the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The spawn rate weight.</returns>
        int GetSpawnRate(int index);

        /// <summary>
        /// Sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <param name="spawn">The new item value.</param>
        void SetSpawn(int index, T spawn);

        /// <summary>
        /// Sets the level range of the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <param name="range">The new level range constraint.</param>
        void SetSpawnRange(int index, IntRange range);

        /// <summary>
        /// Sets the spawn rate of the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <param name="rate">The new spawn rate weight.</param>
        void SetSpawnRate(int index, int rate);

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        void RemoveAt(int index);
    }

    /// <summary>
    /// Non-generic interface for a weighted list of spawnable items with level range constraints.
    /// </summary>
    public interface ISpawnRangeList
    {
        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Inserts an item at the specified index with a level range and spawn rate.
        /// </summary>
        /// <param name="index">The index at which to insert.</param>
        /// <param name="spawn">The item to insert.</param>
        /// <param name="range">The level range where the item can spawn.</param>
        /// <param name="rate">The spawn rate weight.</param>
        void Insert(int index, object spawn, IntRange range, int rate);

        /// <summary>
        /// Adds an item to the list with a level range and spawn rate.
        /// </summary>
        /// <param name="spawn">The item to add.</param>
        /// <param name="range">The level range where the item can spawn.</param>
        /// <param name="rate">The spawn rate weight.</param>
        void Add(object spawn, IntRange range, int rate);

        /// <summary>
        /// Removes all items from the list.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The item at the index.</returns>
        object GetSpawn(int index);

        /// <summary>
        /// Gets the level range of the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The level range constraint.</returns>
        IntRange GetSpawnRange(int index);

        /// <summary>
        /// Gets the spawn rate of the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <returns>The spawn rate weight.</returns>
        int GetSpawnRate(int index);

        /// <summary>
        /// Sets the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <param name="spawn">The new item value.</param>
        void SetSpawn(int index, object spawn);

        /// <summary>
        /// Sets the level range of the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <param name="range">The new level range constraint.</param>
        void SetSpawnRange(int index, IntRange range);

        /// <summary>
        /// Sets the spawn rate of the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <param name="rate">The new spawn rate weight.</param>
        void SetSpawnRate(int index, int rate);

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        void RemoveAt(int index);
    }
}
