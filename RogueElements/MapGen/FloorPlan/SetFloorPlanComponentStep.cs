// <copyright file="SetFloorPlanComponentStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class SetFloorPlanComponentStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        public SetFloorPlanComponentStep()
        {
            this.Components = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        public List<BaseRoomFilter> Filters { get; set; }

        public ComponentCollection Components { get; set; }

        public override void Apply(T map)
        {
            foreach (IRoomPlan plan in map.RoomPlan.GetAllPlans())
            {
                if (!BaseRoomFilter.PassesAllFilters(plan, this.Filters))
                    continue;

                foreach (RoomComponent component in this.Components)
                {
                    plan.Components.Set(component.Clone());
                }
            }
        }
    }
}
