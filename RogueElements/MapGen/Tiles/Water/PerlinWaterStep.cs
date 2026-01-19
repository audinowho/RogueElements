// <copyright file="PerlinWaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Generates water coverage on the map using Perlin noise to create natural-looking terrain.
    /// </summary>
    /// <typeparam name="T">The type of map context that implements <see cref="ITiledGenContext"/>.</typeparam>
    /// <remarks>
    /// This step generates a height map using Perlin noise, then converts all tiles with height values
    /// below a calculated threshold into water tiles, achieving the target water percentage.
    /// </remarks>
    [Serializable]
    public class PerlinWaterStep<T> : WaterStep<T>, IPerlinWaterStep
        where T : class, ITiledGenContext
    {
        private const int BUFFER_SIZE = 5;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerlinWaterStep{T}"/> class.
        /// </summary>
        public PerlinWaterStep()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerlinWaterStep{T}"/> class with the specified parameters.
        /// </summary>
        /// <param name="waterPercent">The target percentage of water coverage.</param>
        /// <param name="complexity">The number of Perlin noise iterations for height map generation.</param>
        /// <param name="terrain">The water terrain tile to place.</param>
        /// <param name="stencil">The stencil that determines eligible placement locations.</param>
        /// <param name="softness">The minimum unit size of water tiles (0 = 1x1, 1 = 2x2, etc.).</param>
        /// <param name="bowl">Whether to apply bowl distortion to prevent edge cutoffs.</param>
        public PerlinWaterStep(RandRange waterPercent, int complexity, ITile terrain, ITerrainStencil<T> stencil, int softness = default, bool bowl = true)
            : base(terrain, stencil)
        {
            this.WaterPercent = waterPercent;
            this.OrderComplexity = complexity;
            this.OrderSoftness = softness;
            this.Bowl = bowl;
        }

        /// <summary>
        /// Gets or sets the number of Perlin noise iterations for height map generation.
        /// Higher values produce more varied heights and more natural-looking terrain.
        /// </summary>
        public int OrderComplexity { get; set; }

        /// <summary>
        /// Gets or sets the minimum unit size of water tiles.
        /// A value of 0 produces 1x1 tiles, 1 produces 2x2 tiles, and so on.
        /// </summary>
        public int OrderSoftness { get; set; }

        /// <summary>
        /// Gets or sets the target percentage of the map to cover with water.
        /// </summary>
        public RandRange WaterPercent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to apply bowl distortion.
        /// When enabled, water values are interpolated upward near map edges to prevent awkward cutoffs.
        /// </summary>
        public bool Bowl { get; set; }

        /// <inheritdoc/>
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
            return string.Format("{0}: {1}% {2}", this.GetType().GetFormattedTypeName(), this.WaterPercent, this.Terrain);
        }
    }
}
