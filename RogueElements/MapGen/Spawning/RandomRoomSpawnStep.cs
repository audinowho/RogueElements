// <copyright file="RandomRoomSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Spawns objects in randomly chosen rooms.
    /// Large rooms have the same probability as small rooms.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TSpawnable"></typeparam>
    [Serializable]
    public class RandomRoomSpawnStep<TGenContext, TSpawnable> : RoomSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        public RandomRoomSpawnStep()
            : base()
        {
        }

        public RandomRoomSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn, bool includeHalls = false)
            : base(spawn)
        {
            this.IncludeHalls = includeHalls;
        }

        /// <summary>
        /// Makes halls eligible for spawn.
        /// </summary>
        public bool IncludeHalls { get; set; }

        public override void DistributeSpawns(TGenContext map, List<TSpawnable> spawns)
        {
            // random per room, not per-tile
            var spawningRooms = new SpawnList<RoomHallIndex>();

            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                if (!BaseRoomFilter.PassesAllFilters(map.RoomPlan.GetRoomPlan(ii), this.Filters))
                    continue;
                spawningRooms.Add(new RoomHallIndex(ii, false), 10);
            }

            if (this.IncludeHalls)
            {
                for (int ii = 0; ii < map.RoomPlan.HallCount; ii++)
                {
                    if (!BaseRoomFilter.PassesAllFilters(map.RoomPlan.GetHallPlan(ii), this.Filters))
                        continue;
                    spawningRooms.Add(new RoomHallIndex(ii, true), 10);
                }
            }

            this.SpawnRandInCandRooms(map, spawningRooms, spawns, 100);
        }

        public override string ToString()
        {
            return string.Format("{0}<{1}>: WithHalls:{2}", this.GetType().Name, typeof(TSpawnable).Name, this.IncludeHalls);
        }
    }
}
