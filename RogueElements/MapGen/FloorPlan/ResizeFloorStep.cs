// <copyright file="ResizeFloorStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Resizes the floor plan.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ResizeFloorStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        public ResizeFloorStep()
        {
        }

        public ResizeFloorStep(Loc addedSize, Dir8 expandDir, Dir8 spaceExpandDir)
        {
            this.AddedSize = addedSize;
            this.ExpandDir = expandDir;
            this.SpaceExpandDir = spaceExpandDir;
        }

        public ResizeFloorStep(Loc addedSize, Dir8 expandDir)
            : this(addedSize, expandDir, Dir8.DownRight)
        {
        }

        /// <summary>
        /// The number of tiles to add to each dimension.
        /// </summary>
        public Loc AddedSize { get; set; }

        /// <summary>
        /// The direction in which to expand the floor space relative to existing rooms.
        /// </summary>
        public Dir8 ExpandDir { get; set; }

        /// <summary>
        /// The direction in which to expand the floor's draw rectangle.
        /// </summary>
        public Dir8 SpaceExpandDir { get; set; }

        public override void Apply(T map)
        {
            map.RoomPlan.Resize(map.RoomPlan.Size + new Loc(this.AddedSize), this.SpaceExpandDir, this.ExpandDir.Reverse());
            GenContextDebug.DebugProgress("Resized Floor");
        }
    }
}
