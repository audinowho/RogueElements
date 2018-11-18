using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements.Examples.Ex1_Tiles
{

    public class MapGenContext : ITiledGenContext
    {
        public Map Map { get; set; }

        public ITile RoomTerrain { get { return new Tile(Map.ROOM_TERRAIN_ID); } }
        public ITile WallTerrain { get { return new Tile(Map.WALL_TERRAIN_ID); } }

        public ITile GetTile(Loc loc) { return Map.Tiles[loc.X][loc.Y]; }
        public bool CanSetTile(Loc loc, ITile tile) { return true; }
        public bool TrySetTile(Loc loc, ITile tile)
        {
            if (!CanSetTile(loc, tile)) return false;
            Map.Tiles[loc.X][loc.Y] = (Tile)tile;
            return true;
        }
        public void SetTile(Loc loc, ITile tile)
        {
            if (!TrySetTile(loc, tile))
                throw new InvalidOperationException("Can't place tile!");
        }

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
            return Map.Tiles[loc.X][loc.Y].ID == 1;
        }

        bool ITiledGenContext.TileBlocked(Loc loc, bool diagonal)
        {
            return Map.Tiles[loc.X][loc.Y].ID == 1;
        }


        public virtual void CreateNew(int width, int height)
        {
            Map.InitializeTiles(width, height);
        }


        public void FinishGen() { }


    }
}
