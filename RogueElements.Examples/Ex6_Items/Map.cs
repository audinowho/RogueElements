using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RogueElements;

namespace RogueElements.Examples.Ex6_Items
{

    public class Map
    {
        public const int WALL_TERRAIN_ID = 0;
        public const int ROOM_TERRAIN_ID = 1;

        public ReRandom Rand;

        public Tile[][] Tiles;
        
        public int Width { get { return Tiles.Length; } }
        public int Height { get { return Tiles[0].Length; } }

        public List<StairsUp> GenEntrances;
        public List<StairsDown> GenExits;
        public List<Item> Items;

        public Map()
        {
            GenEntrances = new List<StairsUp>();
            GenExits = new List<StairsDown>();
            Items = new List<Item>();
        }

        public void InitializeTiles(int width, int height)
        {
            Tiles = new Tile[width][];
            for (int ii = 0; ii < width; ii++)
            {
                Tiles[ii] = new Tile[height];
                for (int jj = 0; jj < height; jj++)
                    Tiles[ii][jj] = new Tile();
            }
        }


    }
}
