// <copyright file="DetectIsolatedStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Detects unreachable walkable tiles on the map and raises an error if any are found.
    /// </summary>
    /// <typeparam name="TGenContext">The type of map context that implements <see cref="ITiledGenContext"/> and <see cref="IViewPlaceableGenContext{T}"/>.</typeparam>
    /// <typeparam name="TEntrance">The type of entrance used to determine the starting point for connectivity checks.</typeparam>
    /// <remarks>
    /// This is a debug step useful for validating that map generation produces fully connected walkable areas.
    /// </remarks>
    [Serializable]
    public class DetectIsolatedStep<TGenContext, TEntrance> : GenStep<TGenContext>
        where TGenContext : class, ITiledGenContext, IViewPlaceableGenContext<TEntrance>
        where TEntrance : IEntrance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetectIsolatedStep{TGenContext, TEntrance}"/> class.
        /// </summary>
        public DetectIsolatedStep()
        {
        }

        /// <inheritdoc/>
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
                (Loc testLoc) => (connectionGrid[testLoc.X - offX][testLoc.Y - offY] || map.TileBlocked(testLoc)),
                (Loc testLoc) => true,
                (Loc fillLoc) => connectionGrid[fillLoc.X - offX][fillLoc.Y - offY] = true,
                map.GetLoc(0));

            for (int xx = offX; xx < offX + lX; xx++)
            {
                for (int yy = offY; yy < offY + lY; yy++)
                {
                    if (!map.TileBlocked(new Loc(xx, yy)) && !connectionGrid[xx - offX][yy - offY])
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

        public override string ToString()
        {
            return string.Format("{0}", this.GetType().GetFormattedTypeName());
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
