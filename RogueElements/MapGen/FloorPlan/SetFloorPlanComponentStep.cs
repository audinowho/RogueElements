// <copyright file="SetFloorPlanComponentStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Takes all rooms in the map's floor plan and gives them a specified component.
    /// These components can be used to identify the room in some way for future filtering.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class SetFloorPlanComponentStep<TGenContext, TTile> : GenStep<TGenContext>
        where TGenContext : class, IFloorPlanGenContext<TTile>
        where TTile : ITile<TTile>
    {
        public SetFloorPlanComponentStep()
        {
            this.Components = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter<TTile>>();
        }

        public List<BaseRoomFilter<TTile>> Filters { get; set; }

        /// <summary>
        /// Components to add.
        /// </summary>
        public ComponentCollection Components { get; set; }

        public override void Apply(TGenContext map)
        {
            foreach (IRoomPlan<TTile> plan in map.RoomPlan.GetAllPlans())
            {
                if (!BaseRoomFilter<TTile>.PassesAllFilters(plan, this.Filters))
                    continue;

                foreach (RoomComponent component in this.Components)
                {
                    plan.Components.Set(component.Clone());
                }
            }
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), this.Components.Count);
        }
    }
}
