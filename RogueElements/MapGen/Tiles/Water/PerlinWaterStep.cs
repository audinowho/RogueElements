// <copyright file="PerlinWaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates a random spread of water on the map. This is achieved by generating a heightContext using Perlin Noise,
    /// then converting all tiles with a height value below the specified threshold to water.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PerlinWaterStep<T> : WaterStep<T>
        where T : class, ITiledGenContext
    {
        private const int BUFFER_SIZE = 5;

        public PerlinWaterStep()
            : base()
        {
        }

        public PerlinWaterStep(RandRange waterPercent, int complexity, ITile terrain, ITerrainStencil<T> stencil, int softness = default, bool bowl = true)
            : base(terrain, stencil)
        {
            this.WaterPercent = waterPercent;
            this.OrderComplexity = complexity;
            this.OrderSoftness = softness;
            this.Bowl = bowl;
        }

        /// <summary>
        /// Determines how many iterations of Perlin noise to generate the heighTContext with. Higher complexity = higher variation of heights and more natural looking terrain.
        /// </summary>
        public int OrderComplexity { get; set; }

        /// <summary>
        /// Determines the smallest unit of water tiles on the map. 0 = 1x1 tile of water, 1 = 2x2 tile of water, etc.
        /// </summary>
        public int OrderSoftness { get; set; }

        /// <summary>
        /// The percent chance of water occurring.
        /// </summary>
        public RandRange WaterPercent { get; set; }

        /// <summary>
        /// Distorts the water such that it becomes like a bowl-shape, preventing awkward cutoffs at the edge of the map.
        /// </summary>
        public bool Bowl { get; set; }

        public override void Apply(T map)
        {
            int waterPercent = this.WaterPercent.Pick(map.Rand);
            if (waterPercent == 0)
                return;

            int depthRange = 0x1 << (this.OrderComplexity + this.OrderSoftness); // aka, 2 ^ degree
            int minWater = waterPercent * map.Width * map.Height / 100;
            int[][] noise = NoiseGen.PerlinNoise(map.Rand, map.Width, map.Height, this.OrderComplexity, this.OrderSoftness);

            if (this.Bowl)
            {
                // distort into a bowl shape
                for (int xx = 0; xx < map.Width; xx++)
                {
                    for (int yy = 0; yy < map.Height; yy++)
                    {
                        // the last BUFFER_SIZE tiles near the edge gradually multiply the actual noise value by smaller numbers
                        int heightPercent = Math.Min(100, Math.Min(Math.Min(xx * 100 / BUFFER_SIZE, yy * 100 / BUFFER_SIZE), Math.Min((map.Width - 1 - xx) * 100 / BUFFER_SIZE, (map.Height - 1 - yy) * 100 / BUFFER_SIZE)));

                        // interpolate UPWARDS to make it like a bowl
                        int correctedNoise = (noise[xx][yy] * heightPercent / 100) + ((depthRange - 1) * (100 - heightPercent) / 100);
                        noise[xx][yy] = correctedNoise;
                    }
                }
            }

            // create histogram of water depths
            int[] depthCount = new int[depthRange];
            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                    depthCount[noise[xx][yy]]++;
            }

            // use the histogram to choose the water level that most closely resembles the percentage desired
            int waterMark = 0;
            int totalDepths = 0;
            for (int ii = 0; ii < depthCount.Length; ii++)
            {
                if (totalDepths + depthCount[ii] >= minWater)
                {
                    if (totalDepths + depthCount[ii] - minWater < minWater - totalDepths)
                        waterMark++;
                    break;
                }

                totalDepths += depthCount[ii];
                waterMark++;
            }

            List<Loc> fillLocs = new List<Loc>();
            for (int xx = 0; xx < map.Width; xx++)
            {
                for (int yy = 0; yy < map.Height; yy++)
                {
                    if (noise[xx][yy] < waterMark)
                        fillLocs.Add(new Loc(xx, yy));
                }
            }

            // permute the locations in case of requirement to preserve paths
            Loc[] shuffleLocs = fillLocs.ToArray();
            NoiseGen.Shuffle(map.Rand, shuffleLocs);

            this.DrawLocs(map, shuffleLocs);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}% {2}", this.GetType().Name, this.WaterPercent, this.Terrain.ToString());
        }
    }
}
