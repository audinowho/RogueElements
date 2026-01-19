// <copyright file="MatchTerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a terrain stencil that tests tiles against a list of allowed tile types.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    [Serializable]
    public class MatchTerrainStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MatchTerrainStencil{T}"/> class.
        /// </summary>
        public MatchTerrainStencil()
        {
            this.MatchTiles = new List<ITile>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchTerrainStencil{T}"/> class with the specified tiles.
        /// </summary>
        /// <param name="negate">Whether to invert the match result.</param>
        /// <param name="tiles">The tile types to match against.</param>
        public MatchTerrainStencil(bool negate, params ITile[] tiles)
            : this()
        {
            this.Negate = negate;
            this.MatchTiles.AddRange(tiles);
        }

        /// <summary>
        /// Gets the list of tile types to match against.
        /// </summary>
        public List<ITile> MatchTiles { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to invert the match result.
        /// </summary>
        public bool Negate { get; set; }

        /// <inheritdoc/>
        public bool Test(T map, Loc loc)
        {
            ITile checkTile = map.GetTile(loc);
            foreach (ITile tile in this.MatchTiles)
            {
                if (checkTile.TileEquivalent(tile))
                    return !this.Negate;
            }

            return this.Negate;
        }

        public override string ToString()
        {
            if (this.MatchTiles.Count == 1)
                return string.Format("Match {0}{1}", this.Negate ? "^" : string.Empty, this.MatchTiles[0].ToString());
            return string.Format("Match {0}[{1}]", this.Negate ? "^" : string.Empty, this.MatchTiles.Count);
        }
    }
}
