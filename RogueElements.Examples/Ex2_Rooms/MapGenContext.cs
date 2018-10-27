using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements.Examples.Ex2_Rooms
{

    public class MapGenContext : ITiledGenContext, IFloorPlanGenContext
    {
        public Map Map { get; set; }

        public ITile RoomTerrain { get { return new Tile(Map.ROOM_TERRAIN_ID); } }
        public ITile WallTerrain { get { return new Tile(Map.WALL_TERRAIN_ID); } }

        public ITile GetTile(Loc loc) { return Map.Tiles[loc.X][loc.Y]; }
        public void SetTile(Loc loc, ITile tile) { Map.Tiles[loc.X][loc.Y] = (Tile)tile; }
        public bool TilesInitialized { get { return Map.Tiles != null; } }

        public int Width { get { return Map.Width; } }
        public int Height { get { return Map.Height; } }


        public IRandom Rand { get { return Map.Rand; } }

        public MapGenContext()
        {
            Map = new Map();
        }
        
        public void InitSeed(ulong seed)
        {
            Map.Rand = new ReRandom(seed);
        }

        bool ITiledGenContext.TileBlocked(Loc loc)
        {
            return Map.Tiles[loc.X][loc.Y].ID == Map.WALL_TERRAIN_ID;
        }

        bool ITiledGenContext.TileBlocked(Loc loc, bool diagonal)
        {
            return Map.Tiles[loc.X][loc.Y].ID == Map.WALL_TERRAIN_ID;
        }


        public virtual void CreateNew(int width, int height)
        {
            Map.InitializeTiles(width, height);
        }


        public void FinishGen() { }


        public void InitPlan(FloorPlan plan)
        {
            RoomPlan = plan;
        }

        public FloorPlan RoomPlan { get; private set; }

    }
}
