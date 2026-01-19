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
    /// <typeparam name="TGenContext">The generation context type, which must support floor plans and placing spawnable entities.</typeparam>
    /// <typeparam name="TSpawnable">The type of spawnable entity to place on the map.</typeparam>
    [Serializable]
    public class RandomRoomSpawnStep<TGenContext, TSpawnable> : RoomSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RandomRoomSpawnStep{TGenContext, TSpawnable}"/> class.
        /// </summary>
        public RandomRoomSpawnStep()
            : base()
        {
            this.SuccessPercent = 100;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomRoomSpawnStep{TGenContext, TSpawnable}"/> class with the specified parameters.
        /// </summary>
        /// <param name="spawn">The spawner that generates the list of items to place.</param>
        /// <param name="successPercent">The percentage to reduce a room's spawn chance after successfully spawning.</param>
        /// <param name="includeHalls">Whether to include halls as eligible spawn locations.</param>
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
        /// Gets or sets a value indicating whether halls are eligible for spawn.
        /// </summary>
        public bool IncludeHalls { get; set; }

        /// <summary>
        /// Distributes spawns by placing each in a randomly selected eligible room.
        /// </summary>
        /// <param name="map">The generation context to place spawns in.</param>
        /// <param name="spawns">The list of spawnable entities to distribute.</param>
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
