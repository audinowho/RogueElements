// <copyright file="FloorStairsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Places entrances and exits in the floor plan with minimum distance requirements.
    /// </summary>
    /// <typeparam name="TGenContext">The generation context type.</typeparam>
    /// <typeparam name="TEntrance">The entrance type implementing <see cref="IEntrance"/>.</typeparam>
    /// <typeparam name="TExit">The exit type implementing <see cref="IExit"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step extends <see cref="BaseFloorStairsStep{TGenContext, TEntrance, TExit}"/> with
    /// distance-based placement. It attempts to place entrances and exits in rooms that are
    /// at least <see cref="MinDistance"/> adjacencies apart, ensuring the player must traverse
    /// a meaningful portion of the floor.
    /// </para>
    /// <para>
    /// When a minimum distance cannot be satisfied, the step falls back to placing in any
    /// available room.
    /// </para>
    /// </remarks>
    [Serializable]
    public class FloorStairsStep<TGenContext, TEntrance, TExit> : BaseFloorStairsStep<TGenContext, TEntrance, TExit>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TEntrance>, IPlaceableGenContext<TExit>
        where TEntrance : IEntrance
        where TExit : IExit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloorStairsStep{TGenContext, TEntrance, TExit}"/> class.
        /// </summary>
        public FloorStairsStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloorStairsStep{TGenContext, TEntrance, TExit}"/> class with a single entrance and exit.
        /// </summary>
        /// <param name="minDistance">The minimum adjacency distance between entrance and exit.</param>
        /// <param name="entrance">The entrance object to place.</param>
        /// <param name="exit">The exit object to place.</param>
        public FloorStairsStep(int minDistance, TEntrance entrance, TExit exit)
            : base(entrance, exit)
        {
            this.MinDistance = minDistance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FloorStairsStep{TGenContext, TEntrance, TExit}"/> class with multiple entrances and exits.
        /// </summary>
        /// <param name="minDistance">The minimum adjacency distance between entrances and exits.</param>
        /// <param name="entrances">The list of entrance objects to place.</param>
        /// <param name="exits">The list of exit objects to place.</param>
        public FloorStairsStep(int minDistance, List<TEntrance> entrances, List<TExit> exits)
            : base(entrances, exits)
        {
            this.MinDistance = minDistance;
        }

        /// <summary>
        /// Gets or sets the minimum distance in room adjacencies between entrances and exits.
        /// </summary>
        public int MinDistance { get; set; }

        /// <summary>
        /// Attempts to choose a location for an entrance or exit while maintaining minimum distance.
        /// </summary>
        /// <typeparam name="T">The spawnable type being placed.</typeparam>
        /// <param name="map">The generation context.</param>
        /// <param name="free_indices">List of room indices not yet used for any entrance or exit.</param>
        /// <param name="used_indices">List of room indices already used. Can be null if not tracking usage.</param>
        /// <returns>A valid location if found; otherwise, null.</returns>
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
