// <copyright file="MapTerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// A filter for determining the eligible tiles for an operation.
    /// Tiles of a certain type are eligible.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class MapTerrainStencil<TGenContext, TTile> : ITerrainStencil<TGenContext, TTile>
        where TGenContext : class, ITiledGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public MapTerrainStencil()
        {
        }

        public MapTerrainStencil(bool room, bool wall, bool blocked, bool not)
        {
            this.Room = room;
            this.Wall = wall;
            this.Blocked = blocked;
            this.Not = not;
        }

        /// <summary>
        /// Allows tiles specified as the map's walkable terrain.
        /// </summary>
        public bool Room { get; private set; }

        /// <summary>
        /// Allows tiles specified as the map's wall terrain.
        /// </summary>
        public bool Wall { get; private set; }

        /// <summary>
        /// Allows tiles specified as the blocked terrain.  This relies on the map's own definition of TileBlocked.
        /// </summary>
        public bool Blocked { get; private set; }

        /// <summary>
        /// Reverses the policy, allowing all tiles EXCEPT the ones selected above.
        /// </summary>
        public bool Not { get; private set; }

        public bool Test(TGenContext map, Loc loc)
        {
            bool result = false;
            if (this.Room && map.RoomTerrain.TileEquivalent(map.GetTile(loc)))
                result = true;
            if (this.Wall && map.WallTerrain.TileEquivalent(map.GetTile(loc)))
                result = true;
            if (this.Blocked && map.TileBlocked(loc))
                result = true;

            if (this.Not)
                return !result;
            else
                return result;
        }

        public override string ToString()
        {
            List<string> listAll = new List<string>();
            if (this.Room)
                listAll.Add(nameof(this.Room));
            if (this.Wall)
                listAll.Add(nameof(this.Wall));
            if (this.Blocked)
                listAll.Add(nameof(this.Blocked));
            if (listAll.Count == 0)
                return string.Format("Match {0}", this.Not ? "anything" : "nothing");
            return string.Format("Match {0}[{1}]", this.Not ? "any EXCEPT" : "any of", string.Join(", ", listAll));
        }
    }
}
