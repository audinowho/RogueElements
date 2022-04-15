// <copyright file="MapTerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
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

        public bool Room { get; private set; }

        public bool Wall { get; private set; }

        public bool Not { get; private set; }

        public bool Test(T map, Loc loc)
        {
            bool result = false;
            if (this.Room && map.GetTile(loc).TileEquivalent(map.RoomTerrain))
                result = true;
            if (this.Wall && map.GetTile(loc).TileEquivalent(map.WallTerrain))
                result = true;

            if (this.Not)
                return !result;
            else
                return result;
        }
    }
}
