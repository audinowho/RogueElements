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
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class MapTerrainStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        public MapTerrainStencil()
        {
        }

        public MapTerrainStencil(bool room, bool wall, bool not)
        {
            this.Room = room;
            this.Wall = wall;
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
        /// Reverses the policy, allowing all tiles EXCEPT the ones selected above.
        /// </summary>
        public bool Not { get; private set; }

        public bool Test(T map, Loc loc)
        {
            bool result = false;
            if (this.Room && map.RoomTerrain.TileEquivalent(map.GetTile(loc)))
                result = true;
            if (this.Wall && map.WallTerrain.TileEquivalent(map.GetTile(loc)))
                result = true;

            if (this.Not)
                return !result;
            else
                return result;
        }
    }
}
