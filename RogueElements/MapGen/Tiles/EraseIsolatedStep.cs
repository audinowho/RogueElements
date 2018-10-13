using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class EraseIsolatedStep<T, E> : GenStep<T> where T : class, ITiledGenContext, IViewPlaceableGenContext<E>
    {
        public int Terrain;

        public EraseIsolatedStep() { }

        public EraseIsolatedStep(int terrain)
        {
            Terrain = terrain;
        }

        public override void Apply(T map)
        {
            bool[][] connectionGrid = new bool[map.Width][];
            for (int xx = 0; xx < map.Width; xx++)
            {
                connectionGrid[xx] = new bool[map.Height];
                for (int yy = 0; yy < map.Height; yy++)
                    connectionGrid[xx][yy] = false;
            }


            Grid.FloodFill(new Rect(0, 0, map.Width, map.Height),
            (Loc testLoc) =>
            {
                bool blocked = map.TileBlocked(testLoc);
                blocked &= (map.Tiles[testLoc.X][testLoc.Y].ID != Terrain);
                return (connectionGrid[testLoc.X][testLoc.Y] || blocked);
            },
            (Loc testLoc) =>
            {
                return true;
            },
            (Loc fillLoc) =>
            {
                connectionGrid[fillLoc.X][fillLoc.Y] = true;
            },
            map.GetLoc(0));

            for (int x = 1; x < map.Width - 1; x++)
            {
                for (int y = 1; y < map.Height - 1; y++)
                {
                    if (map.Tiles[x][y].ID == Terrain && !connectionGrid[x][y])
                        map.Tiles[x][y].ID = map.WallTerrain;
                }
            }
        }

    }
}
