// <copyright file="SetGridPlanComponentStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Adds specified components to rooms and halls in the grid plan for identification and filtering.
    /// </summary>
    /// <typeparam name="T">The map context type, which must implement <see cref="IRoomGridGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// Components are used to tag rooms and halls with metadata that can be used by
    /// subsequent generation steps for filtering or special handling. For example,
    /// a component might mark rooms as eligible for special spawns or as belonging
    /// to a particular zone.
    /// </para>
    /// <para>
    /// This step iterates through all room and hall plans in the grid and adds
    /// copies of the specified components to those that pass the configured filters.
    /// </para>
    /// </remarks>
    /// <seealso cref="RoomComponent"/>
    /// <seealso cref="BaseRoomFilter"/>
    [Serializable]
    public class SetGridPlanComponentStep<T> : GenStep<T>
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetGridPlanComponentStep{T}"/> class.
        /// </summary>
        public SetGridPlanComponentStep()
        {
            this.Components = new ComponentCollection();
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Gets or sets the filters that determine which rooms receive the components.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        /// <summary>
        /// Gets or sets the components to add to matching rooms.
        /// </summary>
        public ComponentCollection Components { get; set; }

        /// <summary>
        /// Adds components to all rooms and halls that pass the configured filters.
        /// </summary>
        /// <param name="map">The map context containing the grid plan.</param>
        public override void Apply(T map)
        {
            foreach (IRoomPlan plan in map.GridPlan.GetAllPlans())
            {
                if (!BaseRoomFilter.PassesAllFilters(plan, this.Filters))
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
