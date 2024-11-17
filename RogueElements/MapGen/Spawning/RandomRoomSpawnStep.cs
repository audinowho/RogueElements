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
            this.SuccessPercent = 100;
        }

        public RandomRoomSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn, int successPercent = 100, bool includeHalls = false)
            : base(spawn)
        {
            this.SuccessPercent = successPercent;
            this.IncludeHalls = includeHalls;
        }

        /// <summary>
        /// The percentage chance to multiply a room's spawning chance when it successfully spawns an item.
        /// 0 means it will never spawn in that room again.
        /// </summary>
        public int SuccessPercent { get; set; }

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
                spawningRooms.Add(new RoomHallIndex(ii, false), 100);
            }

            if (this.IncludeHalls)
            {
                for (int ii = 0; ii < map.RoomPlan.HallCount; ii++)
                {
                    if (!BaseRoomFilter.PassesAllFilters(map.RoomPlan.GetHallPlan(ii), this.Filters))
                        continue;
                    spawningRooms.Add(new RoomHallIndex(ii, true), 100);
                }
            }

            this.SpawnRandInCandRooms(map, spawningRooms, spawns, this.SuccessPercent);
        }
    }
}
