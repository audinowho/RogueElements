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
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MatchTerrainStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        public MatchTerrainStencil()
        {
            this.MatchTiles = new List<ITile>();
        }

        /// <summary>
        /// The allowed tile types.
        /// </summary>
        public List<ITile> MatchTiles { get; private set; }

        public bool Test(T map, Loc loc)
        {
            ITile checkTile = map.GetTile(loc);
            foreach (ITile tile in this.MatchTiles)
            {
                if (checkTile.TileEquivalent(tile))
                    return true;
            }

            return false;
        }
    }
}
