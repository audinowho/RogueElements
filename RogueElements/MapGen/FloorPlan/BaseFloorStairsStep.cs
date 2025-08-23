// <copyright file="BaseFloorStairsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Adds the entrance and exit to the floor.  Is room-conscious.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    /// <typeparam name="TEntrance"></typeparam>
    /// <typeparam name="TExit"></typeparam>
    [Serializable]
    public abstract class BaseFloorStairsStep<TGenContext, TEntrance, TExit> : GenStep<TGenContext>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TEntrance>, IPlaceableGenContext<TExit>
        where TEntrance : IEntrance
        where TExit : IExit
    {
        protected BaseFloorStairsStep()
        {
            this.Entrances = new List<TEntrance>();
            this.Exits = new List<TExit>();
            this.Filters = new List<BaseRoomFilter>();
        }

        protected BaseFloorStairsStep(TEntrance entrance, TExit exit)
        {
            this.Entrances = new List<TEntrance> { entrance };
            this.Exits = new List<TExit> { exit };
            this.Filters = new List<BaseRoomFilter>();
        }

        protected BaseFloorStairsStep(List<TEntrance> entrances, List<TExit> exits)
        {
            this.Entrances = entrances;
            this.Exits = exits;
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// List of entrance objects to spawn.
        /// </summary>
        public List<TEntrance> Entrances { get; }

        /// <summary>
        /// List of exit objects to spawn.
        /// </summary>
        public List<TExit> Exits { get; }

        /// <summary>
        /// Used to filter out rooms that do not make suitable entrances/exits, such as boss rooms.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        public override void Apply(TGenContext map)
        {
            List<int> free_indices = new List<int>();
            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                if (!BaseRoomFilter.PassesAllFilters(map.RoomPlan.GetRoomPlan(ii), this.Filters))
                    continue;
                free_indices.Add(ii);
            }

            List<int> used_indices = new List<int>();

            Loc defaultLoc = Loc.Zero;

            for (int ii = 0; ii < this.Entrances.Count; ii++)
            {
                Loc? start = this.GetOutlet<TEntrance>(map, free_indices, used_indices);

                if (!start.HasValue)
                    start = this.GetOutlet<TEntrance>(map, used_indices, null);
                if (!start.HasValue)
                    start = defaultLoc;

                ((IPlaceableGenContext<TEntrance>)map).PlaceItem(start.Value, this.Entrances[ii]);
                GenContextDebug.DebugProgress(nameof(this.Entrances));
            }

            for (int ii = 0; ii < this.Exits.Count; ii++)
            {
                Loc? end = this.GetOutlet<TExit>(map, free_indices, used_indices);

                if (!end.HasValue)
                    end = this.GetOutlet<TExit>(map, used_indices, null);
                if (!end.HasValue)
                    end = defaultLoc;

                ((IPlaceableGenContext<TExit>)map).PlaceItem(end.Value, this.Exits[ii]);
                GenContextDebug.DebugProgress(nameof(this.Exits));
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: Start: {1} End: {2}", this.GetType().GetFormattedTypeName(), this.Entrances.Count, this.Exits.Count);
        }

        /// <summary>
        /// Attempt to choose an outlet in a room with no entrance/exit, and updates their availability.  If none exists, default to null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="map"></param>
        /// <param name="free_indices"></param>
        /// <param name="used_indices"></param>
        /// <returns></returns>
        protected abstract Loc? GetOutlet<T>(TGenContext map, List<int> free_indices, List<int> used_indices)
            where T : ISpawnable;
    }
}
