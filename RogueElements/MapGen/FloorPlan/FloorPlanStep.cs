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
    public abstract class FloorPlanStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        public abstract void ApplyToPath(IRandom rand, FloorPlan floorPlan);

        public override void Apply(T map)
        {
            this.ApplyToPath(map.Rand, map.RoomPlan);
        }
    }
}
