// <copyright file="TerminalSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Spawns the objects in terminal (dead-end) rooms.
    /// Falls back on normal rooms if all dead-end rooms are taken.
    /// </summary>
    /// <typeparam name="TGenContext">The generation context type, which must support floor plans and placing spawnable entities.</typeparam>
    /// <typeparam name="TSpawnable">The type of spawnable entity to place on the map.</typeparam>
    [Serializable]
    public class TerminalSpawnStep<TGenContext, TSpawnable> : RoomSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalSpawnStep{TGenContext, TSpawnable}"/> class.
        /// </summary>
        public TerminalSpawnStep()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalSpawnStep{TGenContext, TSpawnable}"/> class with the specified parameters.
        /// </summary>
        /// <param name="spawn">The spawner that generates the list of items to place.</param>
        /// <param name="includeHalls">Whether to include halls as eligible spawn locations.</param>
        public TerminalSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn, bool includeHalls = false)
            : base(spawn)
        {
            this.IncludeHalls = includeHalls;
        }

        /// <summary>
        /// Gets or sets a value indicating whether halls are eligible for spawn.
        /// </summary>
        public bool IncludeHalls { get; set; }

        /// <summary>
        /// Distributes spawns by prioritizing terminal (dead-end) rooms, then falling back to other rooms.
        /// </summary>
        /// <param name="map">The generation context to place spawns in.</param>
        /// <param name="spawns">The list of spawnable entities to distribute.</param>
        public override void DistributeSpawns(TGenContext map, List<TSpawnable> spawns)
        {
            // random per room, not per-tile
            var spawningRooms = new SpawnList<RoomHallIndex>();
            var terminalRooms = new SpawnList<RoomHallIndex>();

            // TODO: higher likelihoods for terminals at the ends of longer paths
            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                if (!BaseRoomFilter.PassesAllFilters(map.RoomPlan.GetRoomPlan(ii), this.Filters))
                    continue;
                spawningRooms.Add(new RoomHallIndex(ii, false), 10);
                List<int> adjacent = map.RoomPlan.GetAdjacentRooms(ii);
                if (adjacent.Count == 1)
                    terminalRooms.Add(new RoomHallIndex(ii, false), 10);
            }

            if (this.IncludeHalls)
            {
                for (int ii = 0; ii < map.RoomPlan.HallCount; ii++)
                {
                    if (!BaseRoomFilter.PassesAllFilters(map.RoomPlan.GetHallPlan(ii), this.Filters))
                        continue;
                    spawningRooms.Add(new RoomHallIndex(ii, true), 10);
                    List<RoomHallIndex> adjacent = map.RoomPlan.GetRoomHall(new RoomHallIndex(ii, true)).Adjacents;
                    if (adjacent.Count == 1)
                        terminalRooms.Add(new RoomHallIndex(ii, true), 10);
                }
            }

            // first attempt to spawn in the terminals; remove from terminal list if successful
            this.SpawnRandInCandRooms(map, terminalRooms, spawns, 0);

            this.SpawnRandInCandRooms(map, spawningRooms, spawns, 100);
        }
    }
}
