// <copyright file="DueSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Places items on the map based on how far they are from the entrance of the map.
    /// Distance is measured in the amount of rooms one must travel to reach the room in question.
    /// </summary>
    /// <typeparam name="TGenContext">Type of the MapGenContext.</typeparam>
    /// <typeparam name="TSpawnable">Type of the item to spawn.</typeparam>
    /// <typeparam name="TEntrance">Type of the Map Entrance.</typeparam>
    [Serializable]
    public class DueSpawnStep<TGenContext, TTile, TSpawnable, TEntrance> : RoomSpawnStep<TGenContext, TTile, TSpawnable>
        where TGenContext : class, IFloorPlanGenContext<TTile>, IPlaceableGenContext<TSpawnable>, IViewPlaceableGenContext<TEntrance>
        where TTile : ITile<TTile>
        where TSpawnable : ISpawnable
        where TEntrance : IEntrance
    {
        public DueSpawnStep()
            : base()
        {
        }

        public DueSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn, int successPercent, bool includeHalls = false)
            : base(spawn)
        {
            this.SuccessPercent = successPercent;
            this.IncludeHalls = includeHalls;
        }

        /// <summary>
        /// The percentage chance to multiply a room's spawning chance when it successfully spawns an item.
        /// </summary>
        public int SuccessPercent { get; set; }

        /// <summary>
        /// Makes halls eligible for spawn.
        /// </summary>
        public bool IncludeHalls { get; set; }

        public override void DistributeSpawns(TGenContext map, List<TSpawnable> spawns)
        {
            // gather up all rooms and put in a spawn list
            // rooms that are farther from the start are more likely to have items
            var spawningRooms = new SpawnList<RoomHallIndex>();
            Dictionary<RoomHallIndex, int> roomWeights = new Dictionary<RoomHallIndex, int>();

            // get the start room
            int startRoom = 0;
            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                FloorRoomPlan<TTile> room = map.RoomPlan.GetRoomPlan(ii);
                if (map.RoomPlan.InBounds(room.RoomGen.Draw, map.GetLoc(0)))
                {
                    startRoom = ii;
                    break;
                }
            }

            int maxVal = 1;
            void NodeAct(RoomHallIndex nodeIndex, int distance)
            {
                roomWeights[nodeIndex] = distance + 1;
                maxVal = Math.Max(maxVal, roomWeights[nodeIndex]);
            }

            Graph.TraverseBreadthFirst(new RoomHallIndex(startRoom, false), NodeAct, map.RoomPlan.GetAdjacents);

            int multFactor = int.MaxValue / maxVal / roomWeights.Count;
            foreach (RoomHallIndex idx in roomWeights.Keys)
            {
                IFloorRoomPlan<TTile> room = map.RoomPlan.GetRoomHall(idx);
                if (idx.IsHall && !this.IncludeHalls)
                    continue;
                if (!BaseRoomFilter<TTile>.PassesAllFilters(room, this.Filters))
                    continue;
                if (roomWeights[idx] == 0)
                    continue;
                spawningRooms.Add(idx, roomWeights[idx] * multFactor);
            }

            this.SpawnRandInCandRooms(map, spawningRooms, spawns, this.SuccessPercent);
        }
    }
}
