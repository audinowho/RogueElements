﻿// <copyright file="FloorStairsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    [Serializable]
    public class FloorStairsStep<TGenContext, TEntrance, TExit> : GenStep<TGenContext>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TEntrance>, IPlaceableGenContext<TExit>
        where TEntrance : ISpawnable
        where TExit : ISpawnable
    {
        private readonly List<TEntrance> entrances;
        private readonly List<TExit> exits;

        public FloorStairsStep(TEntrance entrance, TExit exit)
        {
            this.entrances = new List<TEntrance> { entrance };
            this.exits = new List<TExit> { exit };
        }

        public FloorStairsStep(List<TEntrance> entrances, List<TExit> exits)
        {
            this.entrances = entrances;
            this.exits = exits;
        }

        public override void Apply(TGenContext map)
        {
            List<int> room_indices = new List<int>();
            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                room_indices.Add(ii);
            }

            List<int> used_indices = new List<int>();

            Loc defaultLoc = Loc.Zero;

            for (int ii = 0; ii < this.entrances.Count; ii++)
            {
                int startRoom = NextRoom(map.Rand, room_indices, used_indices);
                Loc start = GetOutlet<TEntrance>(map, startRoom);
                if (start == new Loc(-1))
                    start = defaultLoc;
                else
                    defaultLoc = start;
                ((IPlaceableGenContext<TEntrance>)map).PlaceItem(start, this.entrances[ii]);
                GenContextDebug.DebugProgress(nameof(this.entrances));
            }

            for (int ii = 0; ii < this.exits.Count; ii++)
            {
                int endRoom = NextRoom(map.Rand, room_indices, used_indices);
                Loc end = GetOutlet<TExit>(map, endRoom);
                if (end == new Loc(-1))
                    end = defaultLoc;
                ((IPlaceableGenContext<TExit>)map).PlaceItem(end, this.exits[ii]);
                GenContextDebug.DebugProgress(nameof(this.exits));
            }
        }

        /// <summary>
        /// Attempt to choose a room with no entrance/exit, and updates their availability.  If none exists, default to a chosen room.
        /// </summary>
        /// <param name="rand">todo: describe rand parameter on NextRoom</param>
        /// <param name="room_indices">todo: describe room_indices parameter on NextRoom</param>
        /// <param name="used_indices">todo: describe used_indices parameter on NextRoom</param>
        /// <returns></returns>
        private static int NextRoom(IRandom rand, List<int> room_indices, List<int> used_indices)
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

        private static Loc GetOutlet<T>(TGenContext map, int index)
            where T : ISpawnable
        {
            List<Loc> tiles = ((IPlaceableGenContext<T>)map).GetFreeTiles(new Rect(map.RoomPlan.GetRoom(index).Draw.Start, map.RoomPlan.GetRoom(index).Draw.Size));

            if (tiles.Count > 0)
                return tiles[map.Rand.Next(tiles.Count)];

            return -Loc.One;
        }
    }
}
