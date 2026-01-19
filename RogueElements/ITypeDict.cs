// <copyright file="ITypeDict.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a generic type-keyed dictionary interface.
    /// </summary>
    /// <typeparam name="T">The base type of items.</typeparam>
    public interface ITypeDict<T> : IEnumerable<T>, ICollection<T>
    {
        /// <summary>
        /// Determines whether an item of the specified type exists.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if an item of that type exists; otherwise <c>false</c>.</returns>
        bool Contains(Type type);

        /// <summary>
        /// Determines whether an item of the specified type exists.
        /// </summary>
        /// <typeparam name="TK">The type to check.</typeparam>
        /// <returns><c>true</c> if an item of that type exists; otherwise <c>false</c>.</returns>
        bool Contains<TK>()
            where TK : T;

        /// <summary>
        /// Gets the item of the specified type.
        /// </summary>
        /// <typeparam name="TK">The type of item to get.</typeparam>
        /// <returns>The item of that type.</returns>
        TK Get<TK>()
            where TK : T;

        /// <summary>
        /// Removes the item of the specified type.
        /// </summary>
        /// <typeparam name="TK">The type to remove.</typeparam>
        /// <returns><c>true</c> if removed; otherwise <c>false</c>.</returns>
        bool Remove<TK>()
            where TK : T;

        /// <summary>
        /// Gets the item of the specified type.
        /// </summary>
        /// <param name="type">The type of item to get.</param>
        /// <returns>The item of that type.</returns>
        T Get(Type type);

        /// <summary>
        /// Sets an item in the dictionary, keyed by its runtime type.
        /// </summary>
        /// <param name="item">The item to set.</param>
        void Set(T item);

        /// <summary>
        /// Removes the item of the specified type.
        /// </summary>
        /// <param name="type">The type to remove.</param>
        /// <returns><c>true</c> if removed; otherwise <c>false</c>.</returns>
        bool Remove(Type type);
    }

    /// <summary>
    /// Represents a non-generic type-keyed dictionary interface.
    /// </summary>
    public interface ITypeDict : IEnumerable
    {
        /// <summary>
        /// Gets the number of items in the dictionary.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Removes all items from the dictionary.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether an item of the specified type exists.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if an item of that type exists; otherwise <c>false</c>.</returns>
        bool Contains(Type type);

        /// <summary>
        /// Gets the item of the specified type.
        /// </summary>
        /// <param name="type">The type of item to get.</param>
        /// <returns>The item of that type.</returns>
        object Get(Type type);

        /// <summary>
        /// Sets an item in the dictionary, keyed by its runtime type.
        /// </summary>
        /// <param name="item">The item to set.</param>
        void Set(object item);

        /// <summary>
        /// Removes the item of the specified type.
        /// </summary>
        /// <param name="type">The type to remove.</param>
        /// <returns><c>true</c> if removed; otherwise <c>false</c>.</returns>
        bool Remove(Type type);
    }
}
