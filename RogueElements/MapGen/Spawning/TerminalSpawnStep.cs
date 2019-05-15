// <copyright file="TerminalSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class TerminalSpawnStep<TGenContext, TSpawnable> : RoomSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TSpawnable>
        where TSpawnable : ISpawnable
    {
        private readonly bool includeHalls;

        public TerminalSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn, bool includeHalls = false)
            : base(spawn)
        {
            this.includeHalls = includeHalls;
        }

        public override void DistributeSpawns(TGenContext map, List<TSpawnable> spawns)
        {
            // random per room, not per-tile
            var spawningRooms = new SpawnList<RoomHallIndex>();
            var terminalRooms = new SpawnList<RoomHallIndex>();

            // TODO: higher likelihoods for terminals at the ends of longer paths
            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                spawningRooms.Add(new RoomHallIndex(ii, false));
                List<int> adjacent = map.RoomPlan.GetAdjacentRooms(ii);
                if (adjacent.Count == 1)
                    terminalRooms.Add(new RoomHallIndex(ii, false));
            }

            if (this.includeHalls)
            {
                for (int ii = 0; ii < map.RoomPlan.HallCount; ii++)
                {
                    spawningRooms.Add(new RoomHallIndex(ii, true));
                    List<RoomHallIndex> adjacent = map.RoomPlan.GetRoomHall(new RoomHallIndex(ii, true)).Adjacents;
                    if (adjacent.Count == 1)
                        terminalRooms.Add(new RoomHallIndex(ii, true));
                }
            }

            // first attempt to spawn in the terminals; remove from terminal list if successful
            this.SpawnRandInCandRooms(map, terminalRooms, spawns, 0);

            this.SpawnRandInCandRooms(map, spawningRooms, spawns, 100);
        }
    }
}
