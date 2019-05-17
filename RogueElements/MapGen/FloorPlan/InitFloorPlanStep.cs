// <copyright file="InitFloorPlanStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class InitFloorPlanStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        private readonly int width;
        private readonly int height;

        public InitFloorPlanStep(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public override void Apply(T map)
        {
            var floorPlan = new FloorPlan();
            floorPlan.InitSize(new Loc(this.width, this.height));

            map.InitPlan(floorPlan);
        }
    }
}
