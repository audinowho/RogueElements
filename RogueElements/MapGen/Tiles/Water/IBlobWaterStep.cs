// <copyright file="IBlobWaterStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    public interface IBlobWaterStep : IWaterStep
    {
        RandRange Blobs { get; set; }

        IntRange AreaScale { get; set; }

        IntRange GenerateScale { get; set; }
    }
}