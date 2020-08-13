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
    public class DueSpawnStep<TGenContext, TSpawnable, TEntrance> : RoomSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TSpawnable>, IViewPlaceableGenContext<TEntrance>
        where TSpawnable : ISpawnable
        where TEntrance : IEntrance
    {
        public DueSpawnStep()
            : base()
        {
        }

        public DueSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn, int successPercent)
            : base(spawn)
        {
            this.SuccessPercent = successPercent;
        }

        /// <summary>
        /// The percentage chance to multiply a room's spawning chance when it successfully spawns an item.
        /// </summary>
        public int SuccessPercent { get; set; }

        public override void DistributeSpawns(TGenContext map, List<TSpawnable> spawns)
        {
            // gather up all rooms and put in a spawn list
            // rooms that are farther from the start are more likely to have items
            var spawningRooms = new SpawnList<RoomHallIndex>();
            int[] roomWeights = new int[map.RoomPlan.RoomCount];

            // get the start room
            int startRoom = 0;
            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                FloorRoomPlan room = map.RoomPlan.GetRoomPlan(ii);
                if (Collision.InBounds(room.RoomGen.Draw, map.GetLoc(0)))
                {
                    startRoom = ii;
                    break;
                }
            }

            int maxVal = 1;
            void NodeAct(int nodeIndex, int distance)
            {
                roomWeights[nodeIndex] = distance + 1;
                maxVal = Math.Max(maxVal, roomWeights[nodeIndex]);
            }

            List<int> GetAdjacents(int nodeIndex) => map.RoomPlan.GetAdjacentRooms(nodeIndex);

            Graph.TraverseBreadthFirst(roomWeights.Length, startRoom, NodeAct, GetAdjacents);

            int multFactor = int.MaxValue / maxVal / roomWeights.Length;
            for (int ii = 0; ii < roomWeights.Length; ii++)
            {
                FloorRoomPlan room = map.RoomPlan.GetRoomPlan(ii);
                if (!BaseRoomFilter.PassesAllFilters(room, this.Filters))
                    continue;
                if (roomWeights[ii] == 0)
                    continue;
                spawningRooms.Add(new RoomHallIndex(ii, false), roomWeights[ii] * multFactor);
            }

            this.SpawnRandInCandRooms(map, spawningRooms, spawns, this.SuccessPercent);
        }
    }
}
