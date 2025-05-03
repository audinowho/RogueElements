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
    public abstract class GridPlanStep<TGenContext, TTile> : GenStep<TGenContext>
        where TGenContext : class, IRoomGridGenContext<TTile>
        where TTile : ITile<TTile>
    {
        protected GridPlanStep()
        {
        }

        public abstract void ApplyToPath(IRandom rand, GridPlan<TTile> floorPlan);

        public override void Apply(TGenContext map)
        {
            // actual map creation step
            this.ApplyToPath(map.Rand, map.GridPlan);
        }
    }
}
