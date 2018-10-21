using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class EraseIsolatedStep<T> : GenStep<T> where T : class, ITiledGenContext
    {
        public ITile Terrain;

        public EraseIsolatedStep() { }

        public EraseIsolatedStep(ITile terrain)
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


            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    //upon detecting an unmarked room area, fill with connected marks
                    if (map.Tiles[xx][yy].TileEquivalent(map.RoomTerrain) && !connectionGrid[xx][yy])
                    {
                        Grid.FloodFill(new Rect(0, 0, map.Width, map.Height),
                        (Loc testLoc) =>
                        {
                            bool blocked = map.TileBlocked(testLoc);
                            blocked &= !map.Tiles[testLoc.X][testLoc.Y].TileEquivalent(Terrain);
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
                        new Loc(xx,yy));
                    }
                }
            }

            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    if (map.Tiles[xx][yy].TileEquivalent(Terrain) && !connectionGrid[xx][yy])
                        map.Tiles[xx][yy] = map.WallTerrain.Copy();
                }
            }
        }

    }
}
