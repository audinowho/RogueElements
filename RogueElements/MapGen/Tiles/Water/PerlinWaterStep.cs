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
        where T : class, IPerlinWaterStep
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

    }
}
