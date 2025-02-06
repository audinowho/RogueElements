// <copyright file="FloorStairsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Adds the entrance and exit to the floor.  Is room-conscious.
    /// The algorithm will try to place them far away from each other in different rooms.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TEntrance"></typeparam>
    /// <typeparam name="TExit"></typeparam>
    [Serializable]
    public class FloorStairsStep<TGenContext, TEntrance, TExit> : BaseFloorStairsStep<TGenContext, TEntrance, TExit>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TEntrance>, IPlaceableGenContext<TExit>
        where TEntrance : IEntrance
        where TExit : IExit
    {
        public FloorStairsStep()
        {
        }

        public FloorStairsStep(int minDistance, TEntrance entrance, TExit exit)
            : base(entrance, exit)
        {
            this.MinDistance = minDistance;
        }

        public FloorStairsStep(int minDistance, List<TEntrance> entrances, List<TExit> exits)
            : base(entrances, exits)
        {
            this.MinDistance = minDistance;
        }

        /// <summary>
        /// The minimum distance in room adjacencies that starts and ends should be placed from each other.
        /// </summary>
        public int MinDistance { get; set; }

        /// <summary>
        /// Attempt to choose an outlet in a room with no entrance/exit, and updates their availability.  If none exists, default to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="map"></param>
        /// <param name="free_indices"></param>
        /// <param name="used_indices"></param>
        /// <returns></returns>
        protected override Loc? GetOutlet<T>(TGenContext map, List<int> free_indices, List<int> used_indices)
        {
            while (free_indices.Count > 0)
            {
                int roomIndex = map.Rand.Next() % free_indices.Count;
                int startRoom = free_indices[roomIndex];

                List<Loc> tiles = ((IPlaceableGenContext<T>)map).GetFreeTiles(map.RoomPlan.GetRoom(startRoom).Draw);

                if (tiles.Count == 0)
                {
                    // this room is not suitable and never will be, remove it
                    free_indices.RemoveAt(roomIndex);
                    continue;
                }

                Loc start = tiles[map.Rand.Next(tiles.Count)];

                // if we have a used-list, transfer the index over
                if (used_indices != null)
                {
                    free_indices.RemoveAt(roomIndex);
                    used_indices.Add(startRoom);

                    // also transfer all adjacent rooms up to a depth
                    Dictionary<RoomHallIndex, int> roomDistance = new Dictionary<RoomHallIndex, int>();

                    void NodeAct(RoomHallIndex nodeIndex, int distance)
                    {
                        roomDistance[nodeIndex] = distance;

                        if (!nodeIndex.IsHall)
                        {
                            // prefer not to remove by value, but we have no choice
                            free_indices.Remove(nodeIndex.Index);
                            used_indices.Add(nodeIndex.Index);
                        }
                    }

                    List<RoomHallIndex> GetAdjacentsLimited(RoomHallIndex nodeIndex)
                    {
                        if (roomDistance[nodeIndex] + 1 < this.MinDistance)
                            return map.RoomPlan.GetAdjacents(nodeIndex);
                        else
                            return new List<RoomHallIndex>();
                    }

                    Graph.TraverseBreadthFirst(new RoomHallIndex(startRoom, false), NodeAct, GetAdjacentsLimited);
                }

                return start;
            }

            return null;
        }
    }
}
