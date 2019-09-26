// <copyright file="GridPlanStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using RogueElements;

namespace RogueElements
{
    [Serializable]
    public abstract class GridPlanStep<T> : GenStep<T>
        where T : class, IRoomGridGenContext
    {
        protected GridPlanStep()
        {
        }

        public abstract void ApplyToPath(IRandom rand, GridPlan floorPlan);

        public override void Apply(T map)
        {
            // actual map creation step
            this.ApplyToPath(map.Rand, map.GridPlan);
        }
    }
}
