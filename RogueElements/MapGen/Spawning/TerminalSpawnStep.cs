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
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TSpawnable"></typeparam>
    [Serializable]
    public class TerminalSpawnStep<TGenContext, TSpawnable> : RoomSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        public TerminalSpawnStep()
            : base()
        {
        }

        public TerminalSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn, bool includeHalls = false)
            : base(spawn)
        {
            this.IncludeHalls = includeHalls;
        }

        public bool IncludeHalls { get; set; }

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

        public override string ToString()
        {
            return string.Format("{0}<{1}>: WithHalls:{2}", this.GetType().Name, typeof(TSpawnable).Name, this.IncludeHalls);
        }
    }
}
