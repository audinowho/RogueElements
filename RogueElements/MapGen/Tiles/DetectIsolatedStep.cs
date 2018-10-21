using System;

namespace RogueElements
{
    [Serializable]
    public class DetectIsolatedStep<T, E> : GenStep<T>
        where T : class, ITiledGenContext, IViewPlaceableGenContext<E>
    {
        public DetectIsolatedStep() { }

        public override void Apply(T map)
        {
            int offX = 0;
            int offY = 0;
            int lX = map.Width;
            int lY = map.Height;
            bool[][] connectionGrid = new bool[lX][];
            for (int xx = 0; xx < lX; xx++)
            {
                connectionGrid[xx] = new bool[lY];
                for (int yy = 0; yy < lY; yy++)
                    connectionGrid[xx][yy] = false;
            }


            Grid.FloodFill(new Rect(offX, offY, lX, lY),
            (Loc testLoc) =>
            {
                return (connectionGrid[testLoc.X - offX][testLoc.Y - offY] || !map.Tiles[testLoc.X][testLoc.Y].TileEquivalent(map.RoomTerrain));
            },
            (Loc testLoc) =>
            {
                return true;
            },
            (Loc fillLoc) =>
            {
                connectionGrid[fillLoc.X - offX][fillLoc.Y - offY] = true;
            },
            map.GetLoc(0));

            for (int x = offX; x < offX + lX; x++)
            {
                for (int y = offY; y < offY+lY; y++)
                {
                    if (map.Tiles[x][y].TileEquivalent(map.RoomTerrain) && !connectionGrid[x-offX][y-offY])
                    {
                        //throw new Exception("Detected orphaned tile at X" + x + " Y" + y + "!  Seed: " + map.Rand.FirstSeed);
                        Console.WriteLine("Detected orphaned tile at X"+x+" Y"+y+"!  Seed: " + map.Rand.FirstSeed);
                        return;
                    }
                }
            }
        }

    }
}
