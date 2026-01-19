// <copyright file="IPerlinWaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines the interface for Perlin noise-based water generation steps.
    /// </summary>
    public interface IPerlinWaterStep : IWaterStep
    {
        /// <summary>
        /// Gets or sets the number of Perlin noise iterations for height map complexity.
        /// </summary>
        int OrderComplexity { get; set; }

        /// <summary>
        /// Gets or sets the minimum unit size of water tiles.
        /// </summary>
        int OrderSoftness { get; set; }

        /// <summary>
        /// Gets or sets the target percentage of the map to cover with water.
        /// </summary>
        RandRange WaterPercent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to apply bowl distortion to prevent edge cutoffs.
        /// </summary>
        bool Bowl { get; set; }
    }
}
