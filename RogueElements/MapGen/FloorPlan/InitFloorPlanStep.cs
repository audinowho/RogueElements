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
        public InitFloorPlanStep()
        {
        }

        public InitFloorPlanStep(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }

        public int Width { get; set; }

        public int Height { get; set; }

        public override void Apply(T map)
        {
            var floorPlan = new FloorPlan();
            floorPlan.InitSize(new Loc(this.Width, this.Height));

            map.InitPlan(floorPlan);
        }

        public override string ToString()
        {
            return string.Format("{0}: Size:{1}x{2}", this.GetType().Name, this.Width, this.Height);
        }
    }
}
