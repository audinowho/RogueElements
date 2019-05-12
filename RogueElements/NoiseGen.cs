// <copyright file="NoiseGen.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace RogueElements
{

    [Flags]
    public enum CellRule
    {
        None = 0,
        Eq0 = 1,
        Eq1 = 2,
        Eq2 = 4,
        Eq3 = 8,
        Eq4 = 16,
        Eq5 = 32,
        Eq6 = 64,
        Eq7 = 128,
        Eq8 = 256,
        Gte0 = 511,
        Gte1 = 510,
        Gte2 = 508,
        Gte3 = 504,
        Gte4 = 496,
        Gte5 = 480,
        Gte6 = 448,
        Gte7 = 384,
        Gte8 = 256,
        Lt0 = 0,
        Lt1 = 1,
        Lt2 = 3,
        Lt3 = 7,
        Lt4 = 15,
        Lt5 = 31,
        Lt6 = 63,
        Lt7 = 127,
        Lt8 = 255,
        All = 511
    }

    public static class NoiseGen
    {

        /// <summary>
        /// Generates Nth degree perlin noise.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="degrees"></param>
        /// <param name="expandDegrees"></param>
        /// <returns></returns>
        public static int[][] PerlinNoise(IRandom rand, int width, int height, int degrees, int expandDegrees = 0)
        {
            if (degrees > 10)
                degrees = 10;

            int[][] prev_noise = null;
            int[][] noise = null;
            for (int ii = degrees+expandDegrees-1; ii >= 0; ii--)
            {
                int gridWidth = (width - 1) / (int)Math.Pow(2, ii) + 1;
                int gridHeight = (height - 1) / (int)Math.Pow(2, ii) + 1;

                noise = new int[gridWidth][];
                for (int xx = 0; xx < gridWidth; xx++)
                {
                    noise[xx] = new int[gridHeight];
                    for (int yy = 0; yy < gridHeight; yy++)
                    {
                        //Interpolate from the lower resolution iteration, if it exists
                        if (prev_noise != null)
                        {
                            int oldX = xx / 2;
                            int oldY = yy / 2;

                            int newX = (oldX + 1) % prev_noise.Length;
                            int newY = (oldY + 1) % prev_noise[0].Length;

                            int topleft = prev_noise[oldX][oldY];
                            int topright = prev_noise[newX][oldY];
                            int bottomleft = prev_noise[oldX][newY];
                            int bottomright = prev_noise[newX][newY];
                            noise[xx][yy] = BiInterpolate(topleft, topright, bottomleft, bottomright, (xx % 2) * 100 / 2, (yy % 2) * 100 / 2);
                        }
                        //add the new noise (if not merely expanding)
                        if (ii >= expandDegrees)
                            noise[xx][yy] += rand.Next(2) << ii;//aka, ^ ii
                    }
                }
                prev_noise = noise;

            }

            return noise;
        }

        public static int BiInterpolate(int topleft, int topright, int bottomleft, int bottomright, int degreeX, int degreeY)
        {
            return (int)(((topleft * (100 - degreeX) + topright * degreeX) * (100 - degreeY) / 100 + (bottomleft * (100 - degreeX) + bottomright * degreeX) * degreeY / 100) / 100);
        }

        public static int Interpolate(int a, int b, int degree)
        {
            return (int)((a * (100 - degree) + b * degree) / 100);
        }

        public static bool[][] IterateAutomata(bool[][] startGrid, CellRule birth, CellRule survive, int iterations)
        {
            int width = startGrid.Length;
            int height = startGrid[0].Length;

            bool[][] endGrid = new bool[width][];
            for (int ii = 0; ii < width; ii++)
                endGrid[ii] = new bool[height];

            for (int ii = 0; ii < iterations; ii++)
            {
                for (int xx = 0; xx < width; xx++)
                {
                    for (int yy = 0; yy < height; yy++)
                    {
                        int rating = 0;
                        if (checkGrid(xx - 1, yy - 1, startGrid))
                            rating++;
                        if (checkGrid(xx + 1, yy - 1, startGrid))
                            rating++;
                        if (checkGrid(xx - 1, yy + 1, startGrid))
                            rating++;
                        if (checkGrid(xx + 1, yy + 1, startGrid))
                            rating++;
                        if (checkGrid(xx - 1, yy, startGrid))
                            rating++;
                        if (checkGrid(xx + 1, yy, startGrid))
                            rating++;
                        if (checkGrid(xx, yy - 1, startGrid))
                            rating++;
                        if (checkGrid(xx, yy + 1, startGrid))
                            rating++;

                        rating = (0x1 << rating);

                        bool live = startGrid[xx][yy];
                        if (live)
                        {
                            //this is a live cell, use survival rules
                            if ((rating & (int)survive) > 0)
                                endGrid[xx][yy] = true;
                        }
                        else
                        {
                            //this is a dead cell, use birth rules
                            if ((rating & (int)birth) > 0)
                                endGrid[xx][yy] = true;
                        }
                    }
                }

                bool[][] midGrid = startGrid;
                startGrid = endGrid;
                endGrid = midGrid;


                for (int xx = 0; xx < width; xx++)
                {
                    for (int yy = 0; yy < height; yy++)
                        endGrid[xx][yy] = false;
                }
            }

            return startGrid;
        }

        private static bool checkGrid(int x, int y, bool[][] grid)
        {
            if (!Collision.InBounds(grid.Length, grid[0].Length, new Loc(x, y)))
                return false;
            return grid[x][y];
        }



        /// <summary>
        /// Divides a range [min,max] into subdivisions specified by pieces.
        /// Division points count as a tile.  Subdivisions must be at least one tile.
        /// </summary>
        /// <param name="rand"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="pieces"></param>
        /// <returns></returns>
        public static int[] RandomDivide(IRandom rand, int min, int max, int pieces)
        {
            int minSpace = pieces * 2 - 1;
            int maxSpace = max - min + 1;
            if (minSpace > maxSpace)
                throw new ArgumentException("Not enough space to divide!");
            int[] divides = new int[pieces];
            for (int ii = 0; ii < divides.Length; ii++)
                divides[ii]++;

            int fillIn = maxSpace - minSpace;
            for (int ii = 0; ii < fillIn; ii++)
            {
                int chosenGap = rand.Next(pieces);
                divides[chosenGap]++;
            }
            int curTile = min;
            for (int ii = 0; ii < divides.Length; ii++)
            {
                curTile += divides[ii];
                divides[ii] = curTile;
                curTile++;
            }

            return divides;
        }

    }
}
