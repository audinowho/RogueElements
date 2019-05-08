using System;

namespace RogueElements
{
    [Serializable]
    public class DetectIsolatedStairsStep<T, E, F> : GenStep<T>
        where T : class, ITiledGenContext, IViewPlaceableGenContext<E>, IViewPlaceableGenContext<F>
        where E : ISpawnable
        where F : ISpawnable
    {
        public DetectIsolatedStairsStep() { }

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

            //find out if the every entrance can access at least one exit
            for (int ii = 0; ii < ((IViewPlaceableGenContext<E>)map).Count; ii++)
            {
                bool foundExit = false;
                Loc stairLoc = ((IViewPlaceableGenContext<E>)map).GetLoc(ii);
                Grid.FloodFill(new Rect(offX, offY, lX, lY),
                (Loc testLoc) =>
                {
                    return (connectionGrid[testLoc.X - offX][testLoc.Y - offY] || !map.GetTile(testLoc).TileEquivalent(map.RoomTerrain));
                },
                (Loc testLoc) =>
                {
                    return true;
                },
                (Loc fillLoc) =>
                {
                    for (int nn = 0; nn < ((IViewPlaceableGenContext<F>)map).Count; nn++)
                    {
                        if (((IViewPlaceableGenContext<F>)map).GetLoc(nn) == fillLoc)
                            foundExit = true;
                    }
                    connectionGrid[fillLoc.X - offX][fillLoc.Y - offY] = true;
                },
                stairLoc);
                if (!foundExit)
                {
#if DEBUG
                    printGrid(connectionGrid);
                    throw new Exception("Detected orphaned stairs at X" + stairLoc.X + " Y" + stairLoc.Y + "!  Seed: " + map.Rand.FirstSeed);
#else
                        Console.WriteLine("Detected orphaned stairs at X" + stairLoc.X + " Y" + stairLoc.Y + "!  Seed: " + map.Rand.FirstSeed);
                        return;
#endif
                }
            }
        }


        private void printGrid(bool[][] connectionGrid)
        {
            for (int yy = 0; yy < connectionGrid[0].Length; yy++)
            {
                for (int xx = 0; xx < connectionGrid.Length; xx++)
                {
                    System.Diagnostics.Debug.Write(connectionGrid[xx][yy] ? '.' : 'X');
                }
                System.Diagnostics.Debug.Write('\n');
            }
        }
    }
}
