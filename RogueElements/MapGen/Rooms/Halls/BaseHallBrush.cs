// <copyright file="BaseHallBrush.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Base class for brushes that paint hallway tiles.
    /// Brushes define the shape and terrain of hall segments drawn by hall generators.
    /// </summary>
    [Serializable]
    public abstract class BaseHallBrush
    {
        /// <summary>
        /// Gets the size of the brush in tiles, used for alignment purposes.
        /// </summary>
        public abstract Loc Size { get; }

        /// <summary>
        /// Gets the center offset of the brush, used for alignment purposes.
        /// </summary>
        public abstract Loc Center { get; }

        /// <summary>
        /// Creates a deep copy of this brush.
        /// </summary>
        /// <returns>A new instance that is a copy of this brush.</returns>
        public abstract BaseHallBrush Clone();

        /// <summary>
        /// Draws a hallway segment on the map using this brush.
        /// </summary>
        /// <param name="map">The map context to draw on.</param>
        /// <param name="bounds">The bounding rectangle of the hall area.</param>
        /// <param name="ray">The starting point and direction of the hall segment.</param>
        /// <param name="length">The length of the hall segment in tiles.</param>
        public abstract void DrawHallBrush(ITiledGenContext map, Rect bounds, LocRay4 ray, int length);
    }
}
