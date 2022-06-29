// <copyright file="SetGridDefaultsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Takes an existing grid plan and changes some of the rooms into the default room type.
    /// The default room is a single tile in size and effectively acts as a hallway.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class SetGridDefaultsStep<T> : GridPlanStep<T>
        where T : class, IRoomGridGenContext
    {
        public SetGridDefaultsStep()
        {
            this.Filters = new List<BaseRoomFilter>();
        }

        public SetGridDefaultsStep(RandRange defaultRatio, List<BaseRoomFilter> filter)
        {
            this.DefaultRatio = defaultRatio;
            this.Filters = filter;
        }

        /// <summary>
        /// Determines the percentage of eligible rooms that will be turned into default.
        /// </summary>
        public RandRange DefaultRatio { get; set; }

        /// <summary>
        /// Determines which rooms are eligible to be turned into default.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

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
