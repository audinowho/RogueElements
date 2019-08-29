// <copyright file="RandomRoomSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
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

        public bool IncludeHalls { get; set; }

        public override void DistributeSpawns(TGenContext map, List<TSpawnable> spawns)
        {
            // random per room, not per-tile
            var spawningRooms = new SpawnList<RoomHallIndex>();

            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                spawningRooms.Add(new RoomHallIndex(ii, false));
            }

            if (this.IncludeHalls)
            {
                for (int ii = 0; ii < map.RoomPlan.HallCount; ii++)
                {
                    spawningRooms.Add(new RoomHallIndex(ii, true));
                }
            }

            this.SpawnRandInCandRooms(map, spawningRooms, spawns, 100);
        }
    }
}
