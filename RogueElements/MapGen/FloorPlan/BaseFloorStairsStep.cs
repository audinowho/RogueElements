// <copyright file="BaseFloorStairsStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace RogueElements
{
    /// <summary>
    /// Base class for placing entrances and exits in a floor plan-aware manner.
    /// </summary>
    /// <typeparam name="TGenContext">The generation context type.</typeparam>
    /// <typeparam name="TEntrance">The entrance type implementing <see cref="IEntrance"/>.</typeparam>
    /// <typeparam name="TExit">The exit type implementing <see cref="IExit"/>.</typeparam>
    /// <remarks>
    /// This abstract class provides common functionality for placing stairs (or other entrance/exit
    /// objects) while respecting the floor plan's room structure. It ensures entrances and exits
    /// are placed in appropriate rooms and can filter out unsuitable rooms like boss rooms.
    /// </remarks>
    /// <seealso cref="FloorStairsStep{TGenContext, TEntrance, TExit}"/>
    [Serializable]
    public abstract class BaseFloorStairsStep<TGenContext, TEntrance, TExit> : GenStep<TGenContext>
        where TGenContext : class, IFloorPlanGenContext, IPlaceableGenContext<TEntrance>, IPlaceableGenContext<TExit>
        where TEntrance : IEntrance
        where TExit : IExit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFloorStairsStep{TGenContext, TEntrance, TExit}"/> class.
        /// </summary>
        protected BaseFloorStairsStep()
        {
            this.Entrances = new List<TEntrance>();
            this.Exits = new List<TExit>();
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFloorStairsStep{TGenContext, TEntrance, TExit}"/> class with a single entrance and exit.
        /// </summary>
        /// <param name="entrance">The entrance object to place.</param>
        /// <param name="exit">The exit object to place.</param>
        protected BaseFloorStairsStep(TEntrance entrance, TExit exit)
        {
            this.Entrances = new List<TEntrance> { entrance };
            this.Exits = new List<TExit> { exit };
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFloorStairsStep{TGenContext, TEntrance, TExit}"/> class with multiple entrances and exits.
        /// </summary>
        /// <param name="entrances">The list of entrance objects to place.</param>
        /// <param name="exits">The list of exit objects to place.</param>
        protected BaseFloorStairsStep(List<TEntrance> entrances, List<TExit> exits)
        {
            this.Entrances = entrances;
            this.Exits = exits;
            this.Filters = new List<BaseRoomFilter>();
        }

        /// <summary>
        /// Gets the list of entrance objects to spawn.
        /// </summary>
        public List<TEntrance> Entrances { get; }

        /// <summary>
        /// Gets the list of exit objects to spawn.
        /// </summary>
        public List<TExit> Exits { get; }

        /// <summary>
        /// Gets or sets filters to exclude unsuitable rooms from entrance/exit placement.
        /// </summary>
        public List<BaseRoomFilter> Filters { get; set; }

        /// <summary>
        /// Applies this step to place entrances and exits on the map.
        /// </summary>
        /// <param name="map">The generation context.</param>
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

        /// <summary>
        /// Returns a string representation of this step.
        /// </summary>
        /// <returns>A string describing this step's configuration.</returns>
        public override string ToString()
        {
            return string.Format("{0}: Start: {1} End: {2}", this.GetType().GetFormattedTypeName(), this.Entrances.Count, this.Exits.Count);
        }

        /// <summary>
        /// Attempts to choose a location for an entrance or exit, preferring unused rooms.
        /// </summary>
        /// <typeparam name="T">The spawnable type being placed.</typeparam>
        /// <param name="map">The generation context.</param>
        /// <param name="free_indices">List of room indices not yet used for any entrance or exit.</param>
        /// <param name="used_indices">List of room indices already used. Can be null if not tracking usage.</param>
        /// <returns>A valid location if found; otherwise, null.</returns>
        protected abstract Loc? GetOutlet<T>(TGenContext map, List<int> free_indices, List<int> used_indices)
            where T : ISpawnable;
    }
}
