// <copyright file="ClampFloorStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Clamps the floor plan to at least a minimum size, at most a maximum size.
    /// If the bounds of the current roomplan maximum, the size will increase to include them.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ClampFloorStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        public ClampFloorStep()
        {
        }

        public ClampFloorStep(Loc minSize, Loc maxSize)
        {
            this.MinSize = minSize;
            this.MaxSize = maxSize;
        }

        public Loc MinSize { get; set; }

        public Loc MaxSize { get; set; }

        public override void Apply(T map)
        {
            Loc start = map.RoomPlan.Size;
            Loc end = Loc.Zero;
            foreach (IRoomPlan plan in map.RoomPlan.GetAllPlans())
            {
                Rect roomRect = plan.RoomGen.Draw;
                start = new Loc(Math.Min(start.X, roomRect.Start.X), Math.Min(start.Y, roomRect.Start.Y));
                end = new Loc(Math.Max(end.X, roomRect.End.X), Math.Max(end.Y, roomRect.End.Y));
            }

            map.RoomPlan.Resize(end, Dir8.DownRight, Dir8.UpLeft);
            map.RoomPlan.Resize(end, Dir8.UpLeft, Dir8.DownRight);
            GenContextDebug.DebugProgress("Clamped Floor");
        }
    }
}
