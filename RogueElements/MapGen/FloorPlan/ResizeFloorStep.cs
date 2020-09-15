// <copyright file="ResizeFloorStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    [Serializable]
    public class ResizeFloorStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        public ResizeFloorStep()
        {
        }

        public ResizeFloorStep(Loc addedSize, Dir8 expandDir, Dir8 dir)
        {
            this.AddedSize = addedSize;
            this.ExpandDir = expandDir;
            this.AnchorDir = dir;
        }

        public Loc AddedSize { get; set; }

        public Dir8 ExpandDir { get; set; }

        public Dir8 AnchorDir { get; set; }

        public override void Apply(T map)
        {
            map.RoomPlan.Resize(map.RoomPlan.Size + new Loc(this.AddedSize), this.ExpandDir, this.AnchorDir);
            GenContextDebug.DebugProgress("Resized Floor");
        }
    }
}
