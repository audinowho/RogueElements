// <copyright file="MapTerrainStencil.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Provides a terrain stencil that tests tiles based on the map's terrain definitions.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    [Serializable]
    public class MapTerrainStencil<T> : ITerrainStencil<T>
        where T : class, ITiledGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MapTerrainStencil{T}"/> class.
        /// </summary>
        public MapTerrainStencil()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MapTerrainStencil{T}"/> class with the specified filters.
        /// </summary>
        /// <param name="room">Whether to allow tiles matching the map's walkable terrain.</param>
        /// <param name="wall">Whether to allow tiles matching the map's wall terrain.</param>
        /// <param name="blocked">Whether to allow tiles that block movement.</param>
        /// <param name="not">Whether to invert the filter to exclude matching tiles.</param>
        public MapTerrainStencil(bool room, bool wall, bool blocked, bool not)
        {
            this.Room = room;
            this.Wall = wall;
            this.Blocked = blocked;
            this.Not = not;
        }

        /// <summary>
        /// Gets a value indicating whether to allow tiles matching the map's walkable terrain.
        /// </summary>
        public bool Room { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to allow tiles matching the map's wall terrain.
        /// </summary>
        public bool Wall { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to allow tiles that block movement.
        /// This relies on the map's <see cref="ITiledGenContext.TileBlocked(Loc)"/> definition.
        /// </summary>
        public bool Blocked { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to invert the filter.
        /// When set, allows all tiles except those matching the specified criteria.
        /// </summary>
        public bool Not { get; private set; }

        /// <inheritdoc/>
        public bool Test(T map, Loc loc)
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
