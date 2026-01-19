// <copyright file="ResizeFloorStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Expands the floor plan by a specified amount in a given direction.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// This step increases the floor plan dimensions, allowing additional rooms to be placed
    /// in the newly created space. The expansion direction controls where the new space appears
    /// relative to existing rooms. This step has no effect on wrapped floor plans.
    /// </remarks>
    [Serializable]
    public class ResizeFloorStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeFloorStep{T}"/> class with default values.
        /// </summary>
        public ResizeFloorStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeFloorStep{T}"/> class with expansion in a specific direction.
        /// </summary>
        /// <param name="addedSize">The number of tiles to add to each dimension.</param>
        /// <param name="expandDir">The direction in which to add new space relative to existing rooms.</param>
        /// <param name="spaceExpandDir">The direction in which to expand the draw rectangle.</param>
        public ResizeFloorStep(Loc addedSize, Dir8 expandDir, Dir8 spaceExpandDir)
        {
            this.AddedSize = addedSize;
            this.ExpandDir = expandDir;
            this.SpaceExpandDir = spaceExpandDir;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizeFloorStep{T}"/> class with default space expansion.
        /// </summary>
        /// <param name="addedSize">The number of tiles to add to each dimension.</param>
        /// <param name="expandDir">The direction in which to add new space relative to existing rooms.</param>
        public ResizeFloorStep(Loc addedSize, Dir8 expandDir)
            : this(addedSize, expandDir, Dir8.DownRight)
        {
        }

        /// <summary>
        /// Gets or sets the number of tiles to add to each dimension.
        /// </summary>
        public Loc AddedSize { get; set; }

        /// <summary>
        /// Gets or sets the direction in which to expand the floor space relative to existing rooms.
        /// </summary>
        public Dir8 ExpandDir { get; set; }

        /// <summary>
        /// Gets or sets the direction in which to expand the floor's draw rectangle.
        /// </summary>
        public Dir8 SpaceExpandDir { get; set; }

        /// <summary>
        /// Applies this step by resizing the floor plan.
        /// </summary>
        /// <param name="map">The generation context containing the floor plan.</param>
        public override void Apply(T map)
        {
            if (map.RoomPlan.Wrap)
                return;
            map.RoomPlan.Resize(map.RoomPlan.Size + new Loc(this.AddedSize), this.SpaceExpandDir, this.ExpandDir.Reverse());
            GenContextDebug.DebugProgress("Resized Floor");
        }
    }
}
