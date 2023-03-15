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
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BorderTerrainStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        public BorderTerrainStencil()
        {
            this.MatchTiles = new List<ITile>();
        }

        public BorderTerrainStencil(bool negate, params ITile[] tiles)
            : this()
        {
            this.Negate = negate;
            this.MatchTiles.AddRange(tiles);
        }

        /// <summary>
        /// The allowed tile types.
        /// </summary>
        public List<ITile> MatchTiles { get; private set; }

        public bool Negate { get; set; }

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
