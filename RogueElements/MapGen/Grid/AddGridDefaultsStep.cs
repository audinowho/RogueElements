// <copyright file="AddGridDefaultsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class AddGridDefaultsStep<T> : GridPlanStep<T>
        where T : class, IRoomGridGenContext
    {
        public AddGridDefaultsStep()
        {
            this.Filters = new List<BaseRoomFilter>();
        }

        public AddGridDefaultsStep(RandRange defaultRatio, List<BaseRoomFilter> filter)
        {
            this.DefaultRatio = defaultRatio;
            this.Filters = filter;
        }

        public RandRange DefaultRatio { get; set; }

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
