// <copyright file="DueSpawnStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class DueSpawnStep<TGenContext, TSpawnable, TEntrance> : RoomSpawnStep<TGenContext, TSpawnable>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TSpawnable>, IViewPlaceableGenContext<TEntrance>
        where TSpawnable : ISpawnable
        where TEntrance : ISpawnable
    {
        private readonly int successPercent;

        public DueSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn, int successPercent)
            : base(spawn)
        {
            this.successPercent = successPercent;
        }

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
                IRoomGen room = map.RoomPlan.GetRoom(ii);
                if (Collision.InBounds(room.Draw, map.GetLoc(0)))
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
                spawningRooms.Add(new RoomHallIndex(ii, false), roomWeights[ii] * multFactor);
            }

            this.SpawnRandInCandRooms(map, spawningRooms, spawns, this.successPercent);
        }
    }
}
