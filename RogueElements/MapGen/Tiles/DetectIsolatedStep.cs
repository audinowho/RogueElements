// <copyright file="DetectIsolatedStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// A debug step that can be used to generate an error if the map generator created a map with unreachable walkable tiles.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TEntrance"></typeparam>
    [Serializable]
    public class DetectIsolatedStep<TGenContext, TEntrance> : GenStep<TGenContext>
        where TGenContext : class, ITiledGenContext, IViewPlaceableGenContext<IEntrance>
        where TEntrance : IEntrance
    {
        public DetectIsolatedStep()
        {
        }

        public override void Apply(TGenContext map)
        {
            const int offX = 0;
            const int offY = 0;
            int lX = map.Width;
            int lY = map.Height;
            bool[][] connectionGrid = new bool[lX][];
            for (int xx = 0; xx < lX; xx++)
            {
                connectionGrid[xx] = new bool[lY];
                for (int yy = 0; yy < lY; yy++)
                    connectionGrid[xx][yy] = false;
            }

            Grid.FloodFill(
                new Rect(offX, offY, lX, lY),
                (Loc testLoc) => (connectionGrid[testLoc.X - offX][testLoc.Y - offY] || !map.RoomTerrain.TileEquivalent(map.GetTile(testLoc))),
                (Loc testLoc) => true,
                (Loc fillLoc) => connectionGrid[fillLoc.X - offX][fillLoc.Y - offY] = true,
                map.GetLoc(0));

            for (int xx = offX; xx < offX + lX; xx++)
            {
                for (int yy = offY; yy < offY + lY; yy++)
                {
                    if (map.RoomTerrain.TileEquivalent(map.GetTile(new Loc(xx, yy))) && !connectionGrid[xx - offX][yy - offY])
                    {
#if DEBUG
                        PrintGrid(connectionGrid);
                        throw new Exception($"Detected orphaned tile at X{xx} Y{yy}!  Seed: {map.Rand.FirstSeed}");
#else
                        Console.WriteLine($"Detected orphaned tile at X{xx} Y{yy}!  Seed: {map.Rand.FirstSeed}");
                        return;
#endif
                    }
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
