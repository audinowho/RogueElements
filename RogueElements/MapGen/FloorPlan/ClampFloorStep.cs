// <copyright file="ClampFloorStep.cs" company="Audino">
// Copyright (c) Audino
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace RogueElements
{
    /// <summary>
    /// Constrains the floor plan dimensions to specified minimum and maximum bounds.
    /// </summary>
    /// <typeparam name="T">The generation context type, which must implement <see cref="IFloorPlanGenContext"/>.</typeparam>
    /// <remarks>
    /// <para>
    /// This step adjusts the floor plan size to fit within the specified bounds while ensuring
    /// all existing rooms remain contained. If rooms extend beyond the maximum size, the floor
    /// plan expands to include them.
    /// </para>
    /// <para>
    /// The clamping operation anchors at the top-left corner, meaning shrinkage occurs from
    /// the bottom-right direction. This step has no effect on wrapped floor plans.
    /// </para>
    /// </remarks>
    [Serializable]
    public class ClampFloorStep<T> : GenStep<T>
        where T : class, IFloorPlanGenContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClampFloorStep{T}"/> class with default bounds.
        /// </summary>
        public ClampFloorStep()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClampFloorStep{T}"/> class with specified bounds.
        /// </summary>
        /// <param name="minSize">The minimum allowed size for the floor plan.</param>
        /// <param name="maxSize">The maximum allowed size for the floor plan.</param>
        public ClampFloorStep(Loc minSize, Loc maxSize)
        {
            this.MinSize = minSize;
            this.MaxSize = maxSize;
        }

        /// <summary>
        /// Gets or sets the minimum allowed size for the floor plan.
        /// </summary>
        public Loc MinSize { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed size for the floor plan.
        /// </summary>
        public Loc MaxSize { get; set; }

        /// <summary>
        /// Applies this step by clamping the floor plan to the specified bounds.
        /// </summary>
        /// <param name="map">The generation context containing the floor plan.</param>
        public override void Apply(T map)
        {
            if (map.RoomPlan.Wrap)
                return;

            int clampedX = Math.Min(Math.Max(this.MinSize.X, map.RoomPlan.Size.X), this.MaxSize.X);
            int clampedY = Math.Min(Math.Max(this.MinSize.Y, map.RoomPlan.Size.Y), this.MaxSize.Y);

            Loc start = map.RoomPlan.Size;
            Loc end = Loc.Zero;
            foreach (IRoomPlan plan in map.RoomPlan.GetAllPlans())
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
