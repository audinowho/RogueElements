using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RogueElements.Examples.Ex1_Tiles
{

    public class MapGenContext : ITiledGenContext
    {
        public Map Map { get; set; }

        public int RoomTerrain { get { return Map.ROOM_TERRAIN_ID; } }
        public int WallTerrain { get { return Map.WALL_TERRAIN_ID; } }

        ITile[][] ITiledGenContext.Tiles { get { return Map.Tiles; } }

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
