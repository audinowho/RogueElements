// <copyright file="SpecificTilesStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Places a predefined array of tiles onto the map at a specified offset.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    /// <remarks>
    /// This step is useful for placing hand-designed map sections but is not very editor-friendly
    /// due to the raw tile array format.
    /// </remarks>
    [Serializable]
    public class SpecificTilesStep<T> : GenStep<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificTilesStep{T}"/> class.
        /// </summary>
        public SpecificTilesStep()
        {
            this.Tiles = Array.Empty<ITile[]>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificTilesStep{T}"/> class with the specified tiles.
        /// </summary>
        /// <param name="tiles">The 2D array of tiles to place on the map.</param>
        public SpecificTilesStep(ITile[][] tiles)
        {
            this.Tiles = tiles;
            this.Offset = Loc.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpecificTilesStep{T}"/> class with the specified tiles and offset.
        /// </summary>
        /// <param name="tiles">The 2D array of tiles to place on the map.</param>
        /// <param name="offset">The position offset for placing the tiles.</param>
        public SpecificTilesStep(ITile[][] tiles, Loc offset)
        {
            this.Tiles = tiles;
            this.Offset = offset;
        }

        /// <summary>
        /// Gets or sets the 2D array of tiles to place on the map.
        /// </summary>
        public ITile[][] Tiles { get; set; }

        /// <summary>
        /// Gets or sets the position offset for placing the tiles.
        /// </summary>
        public Loc Offset { get; set; }

        /// <inheritdoc/>
        public override void Apply(T map)
        {
            // initialize map array to empty
            // set default map values
            for (int xx = 0; xx < this.Tiles.Length; xx++)
            {
                for (int yy = 0; yy < this.Tiles[0].Length; yy++)
                    map.SetTile(new Loc(this.Offset.X + xx, this.Offset.Y + yy), this.Tiles[xx][yy].Copy());
            }
        }
    }
}
