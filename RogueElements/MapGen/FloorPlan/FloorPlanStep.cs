// <copyright file="FloorPlanStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements
{
    [Serializable]
    public abstract class FloorPlanStep<TGenContext, TTile> : GenStep<TGenContext>
        where TGenContext : class, IFloorPlanGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public abstract void ApplyToPath(IRandom rand, FloorPlan<TTile> floorPlan);

        public override void Apply(TGenContext map)
        {
            this.ApplyToPath(map.Rand, map.RoomPlan);
        }
    }
}
