// <copyright file="IWaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Defines the base interface for water generation steps.
    /// </summary>
    public interface IWaterStep
    {
        /// <summary>
        /// Gets or sets the tile representing the water terrain to place.
        /// </summary>
        ITile Terrain { get; set; }
    }
}
