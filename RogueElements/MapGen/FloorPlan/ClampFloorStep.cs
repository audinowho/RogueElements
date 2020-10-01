// <copyright file="ClampFloorStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    [Serializable]
    public class ClampFloorStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        public ClampFloorStep()
        {
        }

        public ClampFloorStep(Loc minSize, Loc maxSize, Dir8 expandDir, Dir8 dir)
        {
            this.MinSize = minSize;
            this.MaxSize = maxSize;
            this.ExpandDir = expandDir;
            this.AnchorDir = dir;
        }

        public Loc MinSize { get; set; }

        public Loc MaxSize { get; set; }

        public Dir8 ExpandDir { get; set; }

        public Dir8 AnchorDir { get; set; }

        public override void Apply(T map)
        {
            map.RoomPlan.Resize(new Loc(Math.Max(this.MinSize.X, Math.Min(map.RoomPlan.Size.X, this.MaxSize.X)), Math.Max(this.MinSize.Y, Math.Min(map.RoomPlan.Size.Y, this.MaxSize.Y))), this.ExpandDir, this.AnchorDir);
            GenContextDebug.DebugProgress("Clamped Floor");
        }
    }
}
