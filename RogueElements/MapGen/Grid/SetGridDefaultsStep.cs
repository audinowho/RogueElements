// <copyright file="SetGridDefaultsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Converts some rooms to the default single-tile room type, effectively turning them into hallways.
    /// </summary>
    /// <typeparam name="T">The map context type, which must implement <see cref="IRoomGridGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step reduces the number of full rooms in the layout by converting some of them
    /// to minimal single-tile rooms. These default rooms act as connectors or hallways,
    /// creating a more varied layout with larger open areas connected by narrower passages.
    /// </para>
    /// <para>
    /// Only non-terminal rooms (those connected to more than one other room) are eligible
    /// for conversion to prevent dead ends from becoming inaccessible.
    /// </para>
    /// </remarks>
    /// <seealso cref="GridPlanStep{T}"/>
    /// <seealso cref="RoomGenDefault{T}"/>
    [Serializable]
    public class SetGridDefaultsStep<T> : GridPlanStep<T>
        where T : class, IRoomGridGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetGridDefaultsStep{T}"/> class.
        /// </summary>
        public SetGridDefaultsStep()
        {
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetGridDefaultsStep{T}"/> class
        /// with the specified default ratio and filters.
        /// </summary>
        /// <param name="defaultRatio">The percentage range of rooms to convert to default.</param>
        /// <param name="filter">The filters to determine eligible rooms.</param>
        public SetGridDefaultsStep(RandRange defaultRatio, List<BaseRoomFilter> filter)
        {
            this.DefaultRatio = defaultRatio;
            this.Filters = filter;
        }

        /// <summary>
        /// Gets or sets the percentage range of eligible rooms to convert to default.
        /// </summary>
        public RandRange DefaultRatio { get; set; }

        /// <summary>
        /// Gets or sets the filters that determine which rooms are eligible for conversion.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        /// <summary>
        /// Converts a percentage of eligible rooms to the default room type.
        /// </summary>
        /// <param name="rand">The random number generator.</param>
        /// <param name="floorPlan">The grid plan to modify.</param>
        public override void ApplyToPath(IRandom rand, GridPlan floorPlan)
        {
            List<int> candidates = new List<int>();
            for (int ii = 0; ii < floorPlan.RoomCount; ii++)
            {
                if (!BaseRoomFilter.PassesAllFilters(floorPlan.GetRoomPlan(ii), this.Filters))
                    continue;

                List<int> adjacents = floorPlan.GetAdjacentRooms(ii);
                if (adjacents.Count > 1)
                    candidates.Add(ii);
            }

            // our candidates are all rooms except immutables and terminals
            int amountToDefault = this.DefaultRatio.Pick(rand) * candidates.Count / 100;
            for (int ii = 0; ii < amountToDefault; ii++)
            {
                int randIndex = rand.Next(candidates.Count);
                GridRoomPlan plan = floorPlan.GetRoomPlan(candidates[randIndex]);
                plan.RoomGen = new RoomGenDefault<T>();
                plan.PreferHall = true;
                candidates.RemoveAt(randIndex);
                GenContextDebug.DebugProgress("Defaulted Room");
            }
        }
    }
}
