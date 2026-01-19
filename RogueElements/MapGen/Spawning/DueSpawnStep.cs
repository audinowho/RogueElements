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
        /// <summary>
        /// Initializes a new instance of the <see cref="DueSpawnStep{TGenContext, TSpawnable, TEntrance}"/> class.
        /// </summary>
        public DueSpawnStep()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DueSpawnStep{TGenContext, TSpawnable, TEntrance}"/> class with the specified parameters.
        /// </summary>
        /// <param name="spawn">The spawner that generates the list of items to place.</param>
        /// <param name="successPercent">The percentage to reduce a room's spawn chance after successfully spawning.</param>
        /// <param name="includeHalls">Whether to include halls as eligible spawn locations.</param>
        public DueSpawnStep(IStepSpawner<TGenContext, TSpawnable> spawn, int successPercent, bool includeHalls = false)
            : base(spawn)
        {
            this.SuccessPercent = successPercent;
            this.IncludeHalls = includeHalls;
        }

        /// <summary>
        /// Gets or sets the percentage to multiply a room's spawning chance when it successfully spawns an item.
        /// </summary>
        public int SuccessPercent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether halls are eligible for spawn.
        /// </summary>
        public bool IncludeHalls { get; set; }

        /// <summary>
        /// Distributes spawns by weighting rooms based on their distance from the entrance.
        /// Rooms farther from the entrance have higher spawn probability.
        /// </summary>
        /// <param name="map">The generation context to place spawns in.</param>
        /// <param name="spawns">The list of spawnable entities to distribute.</param>
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
                FloorRoomPlan room = map.RoomPlan.GetRoomPlan(ii);
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
                IFloorRoomPlan room = map.RoomPlan.GetRoomHall(idx);
                if (idx.IsHall && !this.IncludeHalls)
                    continue;
                if (!BaseRoomFilter.PassesAllFilters(room, this.Filters))
                    continue;
                if (roomWeights[idx] == 0)
                    continue;
                spawningRooms.Add(idx, roomWeights[idx] * multFactor);
            }

            this.SpawnRandInCandRooms(map, spawningRooms, spawns, this.SuccessPercent);
        }
    }
}
