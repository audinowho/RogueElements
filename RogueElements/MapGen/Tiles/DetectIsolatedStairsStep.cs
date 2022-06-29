// <copyright file="DetectIsolatedStairsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// A debug step that can be used to generate an error if the map generator created an unreachable stairs.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TEntrance"></typeparam>
    /// <typeparam name="TExit"></typeparam>
    [Serializable]
    public class DetectIsolatedStairsStep<TGenContext, TEntrance, TExit> : GenStep<TGenContext>
        where TGenContext : class, ITiledGenContext, IViewPlaceableGenContext<TEntrance>, IViewPlaceableGenContext<TExit>
        where TEntrance : IEntrance
        where TExit : IExit
    {
        public DetectIsolatedStairsStep()
        {
        }

        public override void Apply(TGenContext map)
        {
            int lX = map.Width;
            int lY = map.Height;
            bool[][] connectionGrid = new bool[lX][];
            for (int xx = 0; xx < lX; xx++)
            {
                connectionGrid[xx] = new bool[lY];
                for (int yy = 0; yy < lY; yy++)
                    connectionGrid[xx][yy] = false;
            }

            Rect searchOut = new Rect(0, 0, lX, lY);

            // if the map is wrapped, we have to extend the search rectangle tremendously
            // this doesn't increase memory too much since it is bounded up by the total tiles of the map
            if (map.Wrap)
                searchOut = new Rect(-lX * lY, -lY * lX, lX * ((lY * 2) + 1), lY * ((lX * 2) + 1));

            // find out if the every entrance can access at least one exit
            for (int ii = 0; ii < ((IViewPlaceableGenContext<TEntrance>)map).Count; ii++)
            {
                bool foundExit = false;
                Loc stairLoc = ((IViewPlaceableGenContext<TEntrance>)map).GetLoc(ii);
                Grid.FloodFill(
                    searchOut,
                    (Loc testLoc) =>
                    {
                        testLoc = Loc.Wrap(testLoc, new Loc(lX, lY));
                        return connectionGrid[testLoc.X][testLoc.Y] || !map.RoomTerrain.TileEquivalent(map.GetTile(testLoc));
                    },
                    (Loc testLoc) => true,
                    (Loc fillLoc) =>
                    {
                        fillLoc = Loc.Wrap(fillLoc, new Loc(lX, lY));

                        for (int nn = 0; nn < ((IViewPlaceableGenContext<TExit>)map).Count; nn++)
                        {
                            if (((IViewPlaceableGenContext<TExit>)map).GetLoc(nn) == fillLoc)
                                foundExit = true;
                        }

                        connectionGrid[fillLoc.X][fillLoc.Y] = true;
                    },
                    stairLoc);

                if (!foundExit)
                {
#if DEBUG
                    PrintGrid(connectionGrid);
                    throw new Exception("Detected orphaned stairs at X" + stairLoc.X + " Y" + stairLoc.Y + "!  Seed: " + map.Rand.FirstSeed);
#else
                        Console.WriteLine("Detected orphaned stairs at X" + stairLoc.X + " Y" + stairLoc.Y + "!  Seed: " + map.Rand.FirstSeed);
                        return;
#endif
                }
            }
        }

        private static void PrintGrid(bool[][] connectionGrid)
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
