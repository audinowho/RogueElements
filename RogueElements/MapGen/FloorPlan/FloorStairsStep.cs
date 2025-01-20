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

        public FloorStairsStep(TEntrance entrance, TExit exit)
            : base(entrance, exit)
        {
        }

        public FloorStairsStep(List<TEntrance> entrances, List<TExit> exits)
            : base(entrances, exits)
        {
        }

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
                }

                return start;
            }

            return null;
        }
    }
}
