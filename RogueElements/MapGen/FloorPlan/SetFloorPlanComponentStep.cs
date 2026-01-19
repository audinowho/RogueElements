// <copyright file="SetFloorPlanComponentStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Adds components to rooms in the floor plan for tagging and filtering purposes.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// Components are used to tag rooms with metadata that can be used by subsequent generation
    /// steps for filtering. For example, rooms can be marked as "critical path" or "secret"
    /// so that later steps can treat them differently.
    /// </remarks>
    [Serializable]
    public class SetFloorPlanComponentStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetFloorPlanComponentStep{T}"/> class.
        /// </summary>
        public SetFloorPlanComponentStep()
        {
            this.Components = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Gets or sets filters to select which rooms receive the components.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        /// <summary>
        /// Gets or sets the components to add to matching rooms.
        /// </summary>
        public ComponentCollection Components { get; set; }

        /// <summary>
        /// Applies this step by adding components to filtered rooms.
        /// </summary>
        /// <param name="map">The generation context.</param>
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

        /// <summary>
        /// Returns a string representation of this step.
        /// </summary>
        /// <returns>A string describing this step's configuration.</returns>
        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.GetType().GetFormattedTypeName(), this.Components.Count);
        }
    }
}
