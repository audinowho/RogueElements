// <copyright file="FloorStairsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class FloorStairsStep<T, E, F> : GenStep<T>
        where T : class, IFloorPlanGenContext, IPlaceableGenContext<E>, IPlaceableGenContext<F>
    {
        public List<E> Entrance;
        public List<F> Exit;

        public FloorStairsStep()
        {
            Entrance = new List<E>();
            Exit = new List<F>();
        }

        public FloorStairsStep(E entrance, F exit) : this()
        {
            Entrance.Add(entrance);
            Exit.Add(exit);
        }

        public override void Apply(T map)
        {
            List<int> room_indices = new List<int>();
            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                room_indices.Add(ii);
            }
            List<int> used_indices = new List<int>();

            Loc defaultLoc = new Loc();

            for (int ii = 0; ii < Entrance.Count; ii++)
            {
                int startRoom = NextRoom(map.Rand, room_indices, used_indices);
                Loc start = getOutlet<E>(map, startRoom);
                if (start == new Loc(-1))
                    start = defaultLoc;
                else
                    defaultLoc = start;
                ((IPlaceableGenContext<E>)map).PlaceItem(start, Entrance[ii]);
                GenContextDebug.DebugProgress(nameof(Entrance));
            }

            for (int ii = 0; ii < Exit.Count; ii++)
            {
                int endRoom = NextRoom(map.Rand, room_indices, used_indices);
                Loc end = getOutlet<F>(map, endRoom);
                if (end == new Loc(-1))
                    end = defaultLoc;
                ((IPlaceableGenContext<F>)map).PlaceItem(end, Exit[ii]);
                GenContextDebug.DebugProgress(nameof(Exit));
            }
        }

        /// <summary>
        /// Attempt to choose a room with no entrance/exit, and updates their availability.  If none exists, default to a chosen room.
        /// </summary>
        /// <param name="rand">todo: describe rand parameter on NextRoom</param>
        /// <param name="room_indices">todo: describe room_indices parameter on NextRoom</param>
        /// <param name="used_indices">todo: describe used_indices parameter on NextRoom</param>
        /// <returns></returns>
        private int NextRoom(IRandom rand, List<int> room_indices, List<int> used_indices)
        {
            if (room_indices.Count > 0)
            {
                int roomIndex = rand.Next() % room_indices.Count;
                int startRoom = room_indices[roomIndex];
                used_indices.Add(startRoom);
                room_indices.RemoveAt(roomIndex);
                return startRoom;
            }

            int altIndex = rand.Next() % used_indices.Count;
            return used_indices[altIndex];
        }


        private Loc getOutlet<N>(T map, int index)
        {
            List<Loc> tiles = ((IPlaceableGenContext<N>)map).GetFreeTiles(new Rect(map.RoomPlan.GetRoom(index).Draw.Start, map.RoomPlan.GetRoom(index).Draw.Size));

            if (tiles.Count > 0)
                return tiles[map.Rand.Next(tiles.Count)];

            return new Loc(-1);
        }

    }
}
