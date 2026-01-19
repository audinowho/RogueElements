// <copyright file="IViewPlaceableGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Extends <see cref="IPlaceableGenContext{T}"/> with the ability to query already-placed items.
    /// Provides read access to the collection of spawned entities and their locations.
    /// </summary>
    /// <typeparam name="T">The type of spawnable entity that can be placed and viewed.</typeparam>
    /// <seealso cref="IPlaceableGenContext{T}"/>
    /// <seealso cref="IReplaceableGenContext{T}"/>
    public interface IViewPlaceableGenContext<T> : IPlaceableGenContext<T>
        where T : ISpawnable
    {
        /// <summary>
        /// Gets the number of items currently placed on the map.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to retrieve.</param>
        /// <returns>The spawnable item at the specified index.</returns>
        T GetItem(int index);

        /// <summary>
        /// Gets the location of the item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item whose location to retrieve.</param>
        /// <returns>The location of the item at the specified index.</returns>
        Loc GetLoc(int index);
    }
}
