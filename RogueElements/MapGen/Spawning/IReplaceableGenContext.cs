// <copyright file="IReplaceableGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Extends <see cref="IViewPlaceableGenContext{T}"/> with the ability to modify or remove already-placed items.
    /// Provides write access to the collection of spawned entities.
    /// </summary>
    /// <typeparam name="T">The type of spawnable entity that can be placed, viewed, and replaced.</typeparam>
    /// <seealso cref="IViewPlaceableGenContext{T}"/>
    /// <seealso cref="IPlaceableGenContext{T}"/>
    public interface IReplaceableGenContext<T> : IViewPlaceableGenContext<T>
        where T : ISpawnable
    {
        /// <summary>
        /// Replaces the item at the specified index with a new item.
        /// </summary>
        /// <param name="index">The zero-based index of the item to replace.</param>
        /// <param name="item">The new item to place at the index.</param>
        void SetItem(int index, T item);

        /// <summary>
        /// Removes the item at the specified index from the map.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        void RemoveItemAt(int index);
    }
}
