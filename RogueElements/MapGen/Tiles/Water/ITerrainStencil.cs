// <copyright file="ITerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines a filter for testing individual tile eligibility during terrain operations.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    public interface ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Tests whether a tile at the specified location is eligible for the terrain operation.
        /// </summary>
        /// <param name="map">The map context containing the tile.</param>
        /// <param name="loc">The location of the tile to test.</param>
        /// <returns><c>true</c> if the tile is eligible; otherwise, <c>false</c>.</returns>
        bool Test(T map, Loc loc);
    }
}
