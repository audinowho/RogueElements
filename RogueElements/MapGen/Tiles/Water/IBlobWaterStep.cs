// <copyright file="IBlobWaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines the interface for blob-based water generation steps that place discrete water regions.
    /// </summary>
    public interface IBlobWaterStep : IWaterStep
    {
        /// <summary>
        /// Gets or sets the range for the number of blobs to generate.
        /// </summary>
        RandRange Blobs { get; set; }

        /// <summary>
        /// Gets or sets the acceptable area range for each blob in tiles.
        /// </summary>
        IntRange AreaScale { get; set; }

        /// <summary>
        /// Gets or sets the generation area range used to create each blob.
        /// </summary>
        IntRange GenerateScale { get; set; }
    }
}