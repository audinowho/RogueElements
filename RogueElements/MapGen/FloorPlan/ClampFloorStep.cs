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
    /// Always shrinks in the BottomRight direction, which results in the TopLeft corner remaining constant.
    /// </summary>
    /// <typeparam name="TGenContext"></typeparam>
    [Serializable]
    public class ClampFloorStep<TGenContext, TTile> : GenStep<TGenContext>
        where TGenContext : class, IFloorPlanGenContext<TTile>
        where TTile : ITile<TTile>
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

        public override void Apply(TGenContext map)
        {
            if (map.RoomPlan.Wrap)
                return;

            int clampedX = Math.Min(Math.Max(this.MinSize.X, map.RoomPlan.Size.X), this.MaxSize.X);
            int clampedY = Math.Min(Math.Max(this.MinSize.Y, map.RoomPlan.Size.Y), this.MaxSize.Y);

            Loc start = map.RoomPlan.Size;
            Loc end = Loc.Zero;
            foreach (IRoomPlan<TTile> plan in map.RoomPlan.GetAllPlans())
            {
                Rect roomRect = plan.RoomGen.Draw;
                start = new Loc(Math.Min(start.X, roomRect.Start.X), Math.Min(start.Y, roomRect.Start.Y));
                end = new Loc(Math.Max(end.X, roomRect.End.X), Math.Max(end.Y, roomRect.End.Y));
            }

            // this floor size of end - start is the minimum of which the new map size is allowed
            // increase the size by decreasing the start until 0 or the new size is reached
            // if there is leftover space, increase the size by increasing the end until the new size is reached
            int clampedXDiff = clampedX - (end.X - start.X);
            if (clampedXDiff > 0)
            {
                start.X -= clampedXDiff;
                if (start.X < 0)
                {
                    end.X -= start.X;
                    start.X = 0;
                }
            }

            int clampedYDiff = clampedY - (end.Y - start.Y);
            if (clampedYDiff > 0)
            {
                start.Y -= clampedYDiff;
                if (start.Y < 0)
                {
                    end.Y -= start.Y;
                    start.Y = 0;
                }
            }

            Loc roomSize = end - start;

            map.RoomPlan.Resize(end, Dir8.DownRight, Dir8.UpLeft);
            map.RoomPlan.Resize(roomSize, Dir8.UpLeft, Dir8.DownRight);
            map.RoomPlan.MoveStart(Loc.Zero);
            GenContextDebug.DebugProgress("Clamped Floor");
        }
    }
}
