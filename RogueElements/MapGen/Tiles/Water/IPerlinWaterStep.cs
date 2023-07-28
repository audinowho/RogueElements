// <copyright file="IPerlinWaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IPerlinWaterStep : IWaterStep
    {
        int OrderComplexity { get; set; }

        int OrderSoftness { get; set; }

        RandRange WaterPercent { get; set; }

        bool Bowl { get; set; }
    }
}
