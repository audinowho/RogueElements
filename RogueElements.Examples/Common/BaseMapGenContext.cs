// <copyright file="BaseMapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements.Examples
{
    public abstract class BaseMapGenContext<TMap> : ITiledGenContext
        where TMap : BaseMap, new()
    {
        protected BaseMapGenContext()
        {
            this.Map = new TMap();
        }

        public TMap Map { get; set; }

        public ITile RoomTerrain => new Tile(BaseMap.ROOM_TERRAIN_ID);

        public ITile WallTerrain => new Tile(BaseMap.WALL_TERRAIN_ID);

        public bool TilesInitialized => this.Map.Tiles != null;

        public int Width => this.Map.Width;

        public int Height => this.Map.Height;

        public bool Wrap => false;

        public IRandom Rand => this.Map.Rand;

        public ITile GetTile(Loc loc) => this.Map.Tiles[loc.X][loc.Y];

        public virtual bool CanSetTile(Loc loc, ITile tile) => true;

        public bool TrySetTile(Loc loc, ITile tile)
        {
            if (!this.CanSetTile(loc, tile))
                return false;
            this.Map.Tiles[loc.X][loc.Y] = (Tile)tile;
            return true;
        }

        public void SetTile(Loc loc, ITile tile)
        {
            if (!this.TrySetTile(loc, tile))
                throw new InvalidOperationException("Can't place tile!");
        }

        public void InitSeed(ulong seed)
        {
            this.Map.Rand = new ReRandom(seed);
        }

        bool ITiledGenContext.TileBlocked(Loc loc)
        {
            return this.Map.Tiles[loc.X][loc.Y].ID == BaseMap.WALL_TERRAIN_ID;
        }

        bool ITiledGenContext.TileBlocked(Loc loc, bool diagonal)
        {
            return this.Map.Tiles[loc.X][loc.Y].ID == BaseMap.WALL_TERRAIN_ID;
        }

        public virtual void CreateNew(int width, int height, bool wrap = false)
        {
            this.Map.InitializeTiles(width, height);
        }

        public virtual void FinishGen()
        {
        }
    }
}
