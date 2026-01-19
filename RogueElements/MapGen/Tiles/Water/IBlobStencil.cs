// <copyright file="IBlobStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines a filter for testing blob-wide eligibility during terrain operations.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    public interface IBlobStencil<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Tests whether a blob within the specified bounds is eligible for placement.
        /// </summary>
        /// <param name="map">The map context containing the tiles.</param>
        /// <param name="rect">The bounding rectangle of the blob.</param>
        /// <param name="blobTest">A function that tests whether a location belongs to the blob.</param>
        /// <returns><c>true</c> if the blob placement is eligible; otherwise, <c>false</c>.</returns>
        bool Test(T map, Rect rect, Grid.LocTest blobTest);
    }
}
