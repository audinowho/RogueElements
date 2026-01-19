// <copyright file="BorderTerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a terrain stencil that tests tiles based on their adjacent neighbors.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    /// <remarks>
    /// A tile is eligible if it borders at least one tile matching the specified types.
    /// </remarks>
    [Serializable]
    public class BorderTerrainStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BorderTerrainStencil{T}"/> class.
        /// </summary>
        public BorderTerrainStencil()
        {
            this.MatchTiles = new List<ITile>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BorderTerrainStencil{T}"/> class with the specified tiles.
        /// </summary>
        /// <param name="negate">Whether to invert the match result.</param>
        /// <param name="tiles">The tile types to match against in neighboring tiles.</param>
        public BorderTerrainStencil(bool negate, params ITile[] tiles)
            : this()
        {
            this.Negate = negate;
            this.MatchTiles.AddRange(tiles);
        }

        /// <summary>
        /// Gets the list of tile types to match against in neighboring tiles.
        /// </summary>
        public List<ITile> MatchTiles { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to invert the match result.
        /// </summary>
        public bool Negate { get; set; }

        /// <inheritdoc/>
        public bool Test(T map, Loc loc)
        {
            foreach (Dir8 dir in DirExt.VALID_DIR8)
            {
                Loc moveLoc = loc + dir.GetLoc();
                ITile checkTile = map.GetTile(moveLoc);
                foreach (ITile tile in this.MatchTiles)
                {
                    if (checkTile.TileEquivalent(tile))
                        return !this.Negate;
                }
            }

            return this.Negate;
        }

        public override string ToString()
        {
            if (this.MatchTiles.Count == 1)
                return string.Format("Border of {0}{1}", this.Negate ? "^" : string.Empty, this.MatchTiles[0].ToString());
            return string.Format("Border of {0}[{1}]", this.Negate ? "^" : string.Empty, this.MatchTiles.Count);
        }
    }
}
