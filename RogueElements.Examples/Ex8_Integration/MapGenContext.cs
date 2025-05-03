// <copyright file="MapGenContext.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using RogueSharp;

namespace RogueElements.Examples.Ex8_Integration
{
    public class MapGenContext : ITiledGenContext<CellTile>, IRoomGridGenContext<CellTile>
    {
        public MapGenContext()
        {
            this.Map = new Map();
        }

        public Map Map { get; set; }

        public IRandom Rand { get; private set; }

        public FloorPlan<CellTile> RoomPlan { get; private set; }

        public GridPlan<CellTile> GridPlan { get; private set; }

        public bool TilesInitialized => this.Map.Width > 0 && this.Map.Height > 0;

        public int Width => this.Map.Width;

        public int Height => this.Map.Height;

        public bool Wrap => false;

        public CellTile RoomTerrain => new CellTile(0, 0, true, true, false);

        public CellTile WallTerrain => new CellTile(0, 0, false, false, false);

        public CellTile GetTile(Loc loc) => CellTile.FromCell(this.Map.GetCell(loc.X, loc.Y));

        public bool CanSetTile(Loc loc, CellTile tile) => true;

        public bool TrySetTile(Loc loc, CellTile tile)
        {
            if (!this.CanSetTile(loc, tile))
                return false;
            Cell cell = (Cell)tile;
            this.Map.SetCellProperties(loc.X, loc.Y, cell.IsTransparent, cell.IsWalkable, cell.IsExplored);
            return true;
        }

        public void SetTile(Loc loc, CellTile tile)
        {
            if (!this.TrySetTile(loc, tile))
                throw new InvalidOperationException("Can't place tile!");
        }

        public void InitSeed(ulong seed)
        {
            this.Rand = new ReRandom(seed);
        }

        bool ITiledGenContext<CellTile>.TileBlocked(Loc loc)
        {
            return !this.Map.IsWalkable(loc.X, loc.Y);
        }

        bool ITiledGenContext<CellTile>.TileBlocked(Loc loc, bool diagonal)
        {
            return !this.Map.IsWalkable(loc.X, loc.Y);
        }

        public virtual void CreateNew(int width, int height, bool wrap = false)
        {
            this.Map.Initialize(width, height);
        }

        public void FinishGen()
        {
        }

        public void InitPlan(FloorPlan<CellTile> plan)
        {
            this.RoomPlan = plan;
        }

        public void InitGrid(GridPlan<CellTile> plan)
        {
            this.GridPlan = plan;
        }
    }
}
