// <copyright file="IPlaceableGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Represents a generation context that supports placing spawnable entities at specific locations.
    /// Provides methods to query available placement locations and place items on the map.
    /// </summary>
    /// <typeparam name="T">The type of spawnable entity that can be placed.</typeparam>
    /// <seealso cref="IGenContext"/>
    /// <seealso cref="ISpawnable"/>
    /// <seealso cref="IViewPlaceableGenContext{T}"/>
    public interface IPlaceableGenContext<T> : IGenContext
        where T : ISpawnable
    {
        /// <summary>
        /// Gets all tile locations on the map where items can be placed.
        /// </summary>
        /// <returns>A list of all valid placement locations.</returns>
        List<Loc> GetAllFreeTiles();

        /// <summary>
        /// Gets tile locations within a specified rectangular region where items can be placed.
        /// </summary>
        /// <param name="rect">The rectangular region to search within.</param>
        /// <returns>A list of valid placement locations within the specified rectangle.</returns>
        List<Loc> GetFreeTiles(Rect rect);

        /// <summary>
        /// Determines whether an item can be placed at the specified location.
        /// </summary>
        /// <param name="loc">The location to check.</param>
        /// <returns><c>true</c> if an item can be placed at the location; otherwise, <c>false</c>.</returns>
        bool CanPlaceItem(Loc loc);

        /// <summary>
        /// Places a spawnable item at the specified location on the map.
        /// </summary>
        /// <param name="loc">The location where the item should be placed.</param>
        /// <param name="item">The spawnable item to place.</param>
        void PlaceItem(Loc loc, T item);
    }
}
