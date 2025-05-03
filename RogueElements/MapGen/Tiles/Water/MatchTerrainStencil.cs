// <copyright file="MatchTerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A filter for determining the eligible tiles for an operation.
    /// Tiles in a list of allowed tile types are eligible.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class MatchTerrainStencil<TGenContext, TTile> : ITerrainStencil<TGenContext, TTile>
        where TGenContext : class, ITiledGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public MatchTerrainStencil()
        {
            this.MatchTiles = new List<TTile>();
        }

        public MatchTerrainStencil(bool negate, params TTile[] tiles)
            : this()
        {
            this.Negate = negate;
            this.MatchTiles.AddRange(tiles);
        }

        /// <summary>
        /// The allowed tile types.
        /// </summary>
        public List<TTile> MatchTiles { get; private set; }

        public bool Negate { get; set; }

        public bool Test(TGenContext map, Loc loc)
        {
            TTile checkTile = map.GetTile(loc);
            foreach (TTile tile in this.MatchTiles)
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
