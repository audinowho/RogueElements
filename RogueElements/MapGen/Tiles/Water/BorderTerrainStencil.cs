// <copyright file="BorderTerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A filter for determining the eligible tiles for an operation.
    /// Eligible if bordering a certain tile type.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class BorderTerrainStencil<TGenContext, TTile> : ITerrainStencil<TGenContext, TTile>
        where TGenContext : class, ITiledGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public BorderTerrainStencil()
        {
            this.MatchTiles = new List<TTile>();
        }

        public BorderTerrainStencil(bool negate, params TTile[] tiles)
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
            foreach (Dir8 dir in DirExt.VALID_DIR8)
            {
                Loc moveLoc = loc + dir.GetLoc();
                TTile checkTile = map.GetTile(moveLoc);
                foreach (TTile tile in this.MatchTiles)
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
