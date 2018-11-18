using System;
using System.Collections.Generic;
using RogueSharp;
using RogueSharp.MapCreation;

namespace RogueElements.Examples.Ex7_Integration
{

    public class MapGenContext : ITiledGenContext, IRoomGridGenContext
    {
        public Map Map { get; set; }

        public ITile RoomTerrain { get { return new Cell(0, 0, true, true, false); } }
        public ITile WallTerrain { get { return new Cell(0, 0, false, false, false); } }

        public ITile GetTile(Loc loc) { return (Cell)Map.GetCell(loc.X, loc.Y); }
        public bool CanSetTile(Loc loc, ITile tile)
        {
            return true;
        }
        public bool TrySetTile(Loc loc, ITile tile)
        {
            if (!CanSetTile(loc, tile)) return false;
            Cell cell = (Cell)tile;
            Map.SetCellProperties(loc.X, loc.Y, cell.IsTransparent, cell.IsWalkable, cell.IsExplored);
            return true;
        }
        public void SetTile(Loc loc, ITile tile)
        {
            if (!TrySetTile(loc, tile))
                throw new InvalidOperationException("Can't place tile!");
        }
        public bool TilesInitialized { get { return Map.Width > 0 && Map.Height > 0; } }

        public int Width { get { return Map.Width; } }
        public int Height { get { return Map.Height; } }


        private ReRandom rand;
        public IRandom Rand { get { return rand; } }

        public MapGenContext()
        {
            Map = new Map();
        }
        
        public void InitSeed(ulong seed)
        {
            rand = new ReRandom(seed);
        }

        bool ITiledGenContext.TileBlocked(Loc loc)
        {
            return !Map.IsWalkable(loc.X, loc.Y);
        }

        bool ITiledGenContext.TileBlocked(Loc loc, bool diagonal)
        {
            return !Map.IsWalkable(loc.X, loc.Y);
        }


        public virtual void CreateNew(int width, int height)
        {
            Map.Initialize(width, height);
        }


        public void FinishGen() { }


        public void InitPlan(FloorPlan plan)
        {
            RoomPlan = plan;
        }

        public FloorPlan RoomPlan { get; private set; }


        public void InitGrid(GridPlan plan)
        {
            GridPlan = plan;
        }
        public GridPlan GridPlan { get; private set; }
    }
}
